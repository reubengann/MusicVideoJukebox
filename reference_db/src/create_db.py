import os
from pathlib import Path
import pickle
import re
import time
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
        if track_name is None or recording_artist_name is None:
            print(f"Error: Cannot normalize track {row}")
            raise Exception()
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


def create_db(recreate: bool, start_at: int | None):
    print("Checking server-side tables")
    conn = get_pg()
    dropped_any = False
    did_anything = False

    if os.path.exists(SQLITE_DB):
        print("Deleting old sqlite")
        os.unlink(SQLITE_DB)

    count = 0
    table_steps = [
        ("a_first_pass", "step_1.sql", 174.0),
        ("a_second_pass", "step_1a.sql", 54.6),
        ("a_third_pass", "step_1b.sql", 61.5),
        ("a_ranked_tracks", "step_2.sql", 18.3),
        ("a_rank_1_tracks", "step_3.sql", 3.1),
    ]
    base_time = 10.6 + 18.2 + 121.7 + 14.3 + 5.7 + 4.7
    if start_at and start_at < len(table_steps):
        total_time = base_time + sum([ts[2] for ts in table_steps[start_at - 1 :]])
    else:
        total_time = base_time + sum([ts[2] for ts in table_steps])

    with tqdm.tqdm(total=total_time) as pbar:
        for table_name, table_script, approx_time in table_steps:
            count += 1
            if table_exists(conn, table_name):
                if start_at is not None and count < start_at:
                    tqdm.tqdm.write(f"Keeping {table_name}")
                    continue
                if recreate or did_anything:
                    cur = conn.cursor()
                    tqdm.tqdm.write(f"Dropping and recreating {table_name}")
                    cur.execute(f"DROP TABLE {table_name};")
                    conn.commit()
                    dropped_any = True
                    execute_script(conn, Path(table_script).read_text(encoding="utf-8"))
                    did_anything = True
                else:
                    tqdm.tqdm.write(f"{table_name} already exists")
            else:
                tqdm.tqdm.write(f"Creating {table_name}")
                execute_script(conn, Path(table_script).read_text(encoding="utf-8"))
                did_anything = True
            pbar.update(approx_time)

        dropped_any = True
        if dropped_any:
            tqdm.tqdm.write("Vacuuming")  # 10.6
            old_isolation_level = conn.isolation_level
            conn.set_isolation_level(0)
            cur = conn.cursor()
            cur.execute("VACUUM")
            conn.set_isolation_level(old_isolation_level)
            pbar.update(10.6)

        if did_anything and os.path.exists(PICKLE_FILE):
            tqdm.tqdm.write("Deleting pickle cache")
            os.unlink(PICKLE_FILE)

        if os.path.exists(PICKLE_FILE):
            data = load_from_pickle(PICKLE_FILE)
            tqdm.tqdm.write(f"Data loaded from {PICKLE_FILE}.")
        else:
            tqdm.tqdm.write("Fetching data from PostgreSQL...")  # 18.2
            data = fetch_postgres_data(conn)
            tqdm.tqdm.write(f"Fetched {len(data)} rows.")
            save_to_pickle(data, PICKLE_FILE)
            tqdm.tqdm.write(f"Data saved to {PICKLE_FILE}.")
            pbar.update(18.2)

        # Insert into SQLite
        tqdm.tqdm.write("Inserting flat table into SQLite...")
        sqlite_conn = sqlite3.connect(SQLITE_DB)
        create_flat_table(sqlite_conn)
        populate_flat_table(sqlite_conn, data)  # 121.7
        pbar.update(121.7)
        tqdm.tqdm.write("Creating and populating final tables...")
        create_and_populate_final_tables(sqlite_conn)  # 14.3
        pbar.update(14.3)
        # Drop the flat table
        drop_flat_table(sqlite_conn)  # 5.7
        tqdm.tqdm.write("Dropped flat_tracks table.")
        pbar.update(5.7)
        create_indexes(sqlite_conn)  # 4.7
        pbar.update(4.7)
        sqlite_conn.close()
        tqdm.tqdm.write("Data transfer completed.")
