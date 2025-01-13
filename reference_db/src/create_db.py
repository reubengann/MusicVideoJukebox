import os
from pathlib import Path
import pickle
import re
import unicodedata
import psycopg2
import sqlite3

import tqdm

# PostgreSQL connection details
PG_HOST = "localhost"  # Change if different
PG_PORT = 5432
PG_DB = "musicbrainz_db"
PG_USER = "musicbrainz"
PG_PASSWORD = "musicbrainz"

# SQLite database file
SQLITE_DB = "reference.sqlite"
PICKLE_FILE = "flat_data.pkl"


def get_pg():
    return psycopg2.connect(
        host=PG_HOST, port=PG_PORT, dbname=PG_DB, user=PG_USER, password=PG_PASSWORD
    )


def fetch_postgres_data(conn):
    query = """
SELECT 
    track_id, 
    track_name, 
    recording_artist_id, 
    recording_artist_name, 
    album_id, 
    album_title, 
    release_year
FROM a_rank_1_tracks;
"""
    cur = conn.cursor()
    cur.execute(query)
    rows = cur.fetchall()
    cur.close()
    conn.close()
    return rows


def save_to_pickle(data, pickle_file):
    """Save data to a pickle file."""
    with open(pickle_file, "wb") as f:
        pickle.dump(data, f)


def load_from_pickle(pickle_file):
    """Load data from a pickle file."""
    with open(pickle_file, "rb") as f:
        data = pickle.load(f)
    return data


def normalize_name(name):
    """
    Normalize names by converting to lowercase, removing special characters,
    and trimming unnecessary spaces.
    """
    # Normalize Unicode to decompose accents
    name = unicodedata.normalize("NFD", name)
    # Remove accents by excluding characters in the 'Mn' (Mark, Nonspacing) category
    name = "".join(c for c in name if unicodedata.category(c) != "Mn")
    # Remove 'the' at the beginning, normalize spaces, and remove special characters
    name = name.strip().lower()
    name = re.sub(r"^the\s+", "", name)  # Remove leading 'the'
    name = re.sub(r"[^a-z0-9\s]", "", name)  # Remove special characters except spaces
    name = re.sub(r"\s+", " ", name)  # Normalize spaces
    return name


def create_flat_table(conn):
    """Create the flat_data table in SQLite."""
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS flat_tracks (
            track_id INTEGER,
            track_name TEXT NOT NULL,
            recording_artist_id INTEGER NOT NULL,
            recording_artist_name TEXT NOT NULL,
            album_id INTEGER NOT NULL,
            album_title TEXT,
            release_year INTEGER,
            artist_title TEXT NOT NULL, -- Normalized artist - title
            PRIMARY KEY (track_id)
        );
    """
    )
    conn.commit()


def populate_flat_table(conn, data):
    cursor = conn.cursor()
    # Insert data into the flat table
    for row in tqdm.tqdm(data):
        (
            track_id,
            track_name,
            recording_artist_id,
            recording_artist_name,
            album_id,
            album_title,
            release_year,
        ) = row

        # Normalize track and artist names
        normalized_track_name = normalize_name(track_name)
        normalized_artist_name = normalize_name(recording_artist_name)
        artist_title = f"{normalized_artist_name} - {normalized_track_name}"

        # Insert into SQLite
        cursor.execute(
            """
            INSERT OR IGNORE INTO flat_tracks (
                track_id, track_name, recording_artist_id, recording_artist_name, 
                album_id, album_title, release_year, artist_title
            )
            VALUES (?, ?, ?, ?, ?, ?, ?, ?);
        """,
            (
                track_id,
                track_name,
                recording_artist_id,
                recording_artist_name,
                album_id,
                album_title,
                release_year,
                artist_title,
            ),
        )

    conn.commit()


def create_and_populate_final_tables(conn):

    cursor = conn.cursor()
    # Create tables
    cursor.execute(
        """
        CREATE TABLE IF NOT EXISTS artists (
            artist_id INTEGER PRIMARY KEY,
            artist_name TEXT
        );
    """
    )

    # Populate Artists Table
    cursor.execute(
        """
        INSERT OR IGNORE INTO artists (artist_id, artist_name)
        SELECT DISTINCT recording_artist_id, recording_artist_name
        FROM flat_tracks;
    """
    )

    # Create Albums Table
    cursor.execute(
        """
        CREATE TABLE IF NOT EXISTS albums (
            album_id INTEGER PRIMARY KEY,
            album_title TEXT,
            release_year INTEGER
        );
    """
    )

    # Populate Albums Table
    cursor.execute(
        """
        INSERT OR IGNORE INTO albums (album_id, album_title, release_year)
        SELECT DISTINCT album_id, album_title, release_year
        FROM flat_tracks;
    """
    )

    # Create Tracks Table
    cursor.execute(
        """
        CREATE TABLE IF NOT EXISTS tracks (
            track_id INTEGER PRIMARY KEY,
            track_name TEXT,
            artist_id INTEGER,
            album_id INTEGER,
            artist_title TEXT,
            FOREIGN KEY (artist_id) REFERENCES artists (artist_id),
            FOREIGN KEY (album_id) REFERENCES albums (album_id)
        );
    """
    )

    # Populate Tracks Table
    cursor.execute(
        """
        INSERT OR IGNORE INTO tracks (
            track_id, track_name, artist_id, album_id, artist_title
        )
        SELECT 
            track_id, 
            track_name, 
            recording_artist_id AS artist_id, 
            album_id, 
            artist_title
        FROM flat_tracks;
    """
    )

    conn.commit()


def drop_flat_table(conn):
    """Drop the flat_tracks table."""
    conn.execute("DROP TABLE flat_tracks;")
    conn.commit()
    conn.execute("VACUUM;")
    print("Dropped flat_tracks table.")


def create_indexes(conn):
    conn.execute(
        "CREATE INDEX IF NOT EXISTS idx_tracks_artist_title ON tracks (artist_title);"
    )
    conn.commit()


def execute_script(conn: psycopg2.extensions.connection, sql_script: str):
    cursor = conn.cursor()
    cursor.execute(sql_script)
    conn.commit()


def table_exists(conn, table_name):
    """
    Check if a table exists in the SQLite database.
    """
    cursor = conn.cursor()
    cursor.execute(
        """
        SELECT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_tables
            WHERE schemaname = 'musicbrainz' AND tablename = %s
        );
        """,
        (table_name,),
    )
    result = cursor.fetchone()
    return result[0]


def create_db(recreate: bool, keep_first_pass: bool):
    print("Checking server-side tables")
    conn = get_pg()
    dropped_any = False
    did_anything = False

    if recreate and os.path.exists(SQLITE_DB):
        print("Deleting old sqlite")
        os.unlink(SQLITE_DB)

    for table_name, table_script in [
        ("a_first_pass", "step_1.sql"),
        ("a_second_pass", "step_1a.sql"),
        ("a_third_pass", "step_1b.sql"),
        ("a_ranked_tracks", "step_2.sql"),
        ("a_rank_1_tracks", "step_3.sql"),
    ]:
        if table_name == "a_first_pass" and recreate and keep_first_pass:
            print("Keeping a_first_pass")
            continue
        if table_exists(conn, table_name):
            if recreate:
                cur = conn.cursor()
                print(f"Dropping and recreating {table_name}")
                cur.execute(f"DROP TABLE {table_name};")
                conn.commit()
                dropped_any = True
                execute_script(conn, Path(table_script).read_text(encoding="utf-8"))
                did_anything = True
            else:
                print(f"{table_name} already exists")
        else:
            print(f"Creating {table_name}")
            execute_script(conn, Path(table_script).read_text(encoding="utf-8"))
            did_anything = True

    dropped_any = True
    if dropped_any:
        print("Vacuuming")
        old_isolation_level = conn.isolation_level
        conn.set_isolation_level(0)
        cur = conn.cursor()
        cur.execute("VACUUM")
        conn.set_isolation_level(old_isolation_level)

    if did_anything and os.path.exists(PICKLE_FILE):
        print("Deleting pickle cache")
        os.unlink(PICKLE_FILE)

    if os.path.exists(PICKLE_FILE):
        data = load_from_pickle(PICKLE_FILE)
        print(f"Data loaded from {PICKLE_FILE}.")
    else:
        print("Fetching data from PostgreSQL...")
        data = fetch_postgres_data(conn)
        print(f"Fetched {len(data)} rows.")
        save_to_pickle(data, PICKLE_FILE)
        print(f"Data saved to {PICKLE_FILE}.")

    # Insert into SQLite
    print("Inserting flat table into SQLite...")
    sqlite_conn = sqlite3.connect(SQLITE_DB)
    create_flat_table(sqlite_conn)
    populate_flat_table(sqlite_conn, data)
    print("Creating and populating final tables...")
    create_and_populate_final_tables(sqlite_conn)
    # Drop the flat table
    drop_flat_table(sqlite_conn)
    create_indexes(sqlite_conn)
    sqlite_conn.close()
    print("Data transfer completed.")
