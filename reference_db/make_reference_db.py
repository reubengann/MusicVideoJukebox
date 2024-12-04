import os
import pickle
import psycopg2
import sqlite3

# PostgreSQL connection details
PG_HOST = "localhost"  # Change if different
PG_PORT = 5432
PG_DB = "musicbrainz_db"
PG_USER = "musicbrainz"
PG_PASSWORD = "musicbrainz"

# SQLite database file
SQLITE_DB = "reference.sqlite"
FLAT_QUERY = "flat_query.sql"
PICKLE_FILE = "flat_data.pkl"


def fetch_postgres_data(query):
    """Fetch data from PostgreSQL."""
    conn = psycopg2.connect(
        host=PG_HOST, port=PG_PORT, dbname=PG_DB, user=PG_USER, password=PG_PASSWORD
    )
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
    print(f"Data saved to {pickle_file}.")


def load_from_pickle(pickle_file):
    """Load data from a pickle file."""
    with open(pickle_file, "rb") as f:
        data = pickle.load(f)
    print(f"Data loaded from {pickle_file}.")
    return data


def create_flat_table(conn):
    """Create the flat_data table in SQLite."""
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS flat_data (
    track_id INTEGER PRIMARY KEY,
    track_name TEXT,
    artist_id INTEGER,
    artist_name TEXT,
    album_id INTEGER,
    album_title TEXT,
    primary_type_id INTEGER,
    primary_type_name TEXT,
    secondary_type_id INTEGER,
    secondary_type_name TEXT,
    first_release_date_year INTEGER
);
    """
    )
    conn.commit()


def populate_flat_table(conn, data):
    """Populate the flat_data table in SQLite."""
    conn.executemany(
        """
        INSERT INTO flat_data (
            track_id, track_name, artist_id, artist_name, album_id, album_title,
            primary_type_id, primary_type_name, secondary_type_id, secondary_type_name, first_release_date_year
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
    """,
        data,
    )
    conn.commit()


def populate_type_tables(conn):
    """Populate the primary_types and secondary_types tables."""
    conn.execute(
        """
        INSERT INTO primary_types (primary_type_id, primary_type_name)
        SELECT DISTINCT primary_type_id, primary_type_name
        FROM flat_data
        WHERE primary_type_id IS NOT NULL;
    """
    )
    conn.execute(
        """
        INSERT INTO secondary_types (secondary_type_id, secondary_type_name)
        SELECT DISTINCT secondary_type_id, seconary_type_name
        FROM flat_data
        WHERE secondary_type_id IS NOT NULL;
    """
    )
    conn.commit()


def create_type_tables(conn):
    """Create the primary_types and secondary_types tables."""
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS primary_types (
            primary_type_id INTEGER PRIMARY KEY,
            primary_type_name TEXT
        );
    """
    )
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS secondary_types (
            secondary_type_id INTEGER PRIMARY KEY,
            secondary_type_name TEXT
        );
    """
    )
    conn.commit()


def create_and_populate_final_tables(conn):

    # Create tables
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS artists (
            artist_id INTEGER PRIMARY KEY,
            artist_name TEXT
        );
    """
    )
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS albums (
            album_id INTEGER PRIMARY KEY,
            artist_id INTEGER,
            album_title TEXT,
            primary_type_id INTEGER,
            secondary_type_id INTEGER,
            first_release_date_year INTEGER
        );
    """
    )
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS tracks (
            track_id INTEGER PRIMARY KEY,
            track_name TEXT,
            album_id INTEGER
        );
    """
    )

    conn.commit()

    # Populate tables
    conn.execute(
        """
        INSERT INTO artists (artist_id, artist_name)
        SELECT DISTINCT artist_id, artist_name
        FROM flat_data;
    """
    )
    conn.execute(
        """
        INSERT INTO albums (album_id, artist_id, album_title, primary_type_id, secondary_type_id, first_release_date_year)
SELECT album_id, artist_id, album_title, primary_type_id, 
       MIN(secondary_type_id) AS secondary_type_id, -- Pick the smallest secondary_type_id
       first_release_date_year
FROM flat_data
GROUP BY album_id, artist_id, album_title, primary_type_id, first_release_date_year;
    """
    )
    conn.execute(
        """
        INSERT INTO tracks (track_id, track_name, album_id)
        SELECT DISTINCT track_id, track_name, album_id
        FROM flat_data;
    """
    )
    conn.commit()


def drop_flat_table(conn):
    """Drop the flat_data table."""
    conn.execute("DROP TABLE flat_data;")
    conn.commit()
    conn.execute("VACUUM;")
    print("Dropped flat_data table.")


def create_indexes(conn):
    """Create indexes to optimize SQLite queries."""
    conn.execute("CREATE INDEX idx_artists_artist_name ON artists (artist_name);")
    conn.execute("CREATE INDEX idx_tracks_track_name ON tracks (track_name);")
    conn.commit()


def main():
    # Check for pickle file
    if os.path.exists(PICKLE_FILE):
        # Load from pickle
        data = load_from_pickle(PICKLE_FILE)
    else:
        # Load query and fetch data from PostgreSQL
        with open(FLAT_QUERY, "r") as file:
            query = file.read()
        print("Fetching data from PostgreSQL...")
        data = fetch_postgres_data(query)
        print(f"Fetched {len(data)} rows.")
        save_to_pickle(data, PICKLE_FILE)

    # Insert into SQLite
    print("Inserting data into SQLite...")
    sqlite_conn = sqlite3.connect(SQLITE_DB)
    create_flat_table(sqlite_conn)
    populate_flat_table(sqlite_conn, data)
    print("Creating and populating final tables...")
    create_and_populate_final_tables(sqlite_conn)
    populate_type_tables(sqlite_conn)
    # Drop the flat table
    drop_flat_table(sqlite_conn)
    create_indexes(sqlite_conn)
    sqlite_conn.close()
    print("Data transfer completed.")


if __name__ == "__main__":
    main()
