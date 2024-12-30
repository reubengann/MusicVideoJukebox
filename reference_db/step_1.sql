CREATE TABLE a_first_pass (
    id SERIAL PRIMARY KEY, -- Auto-incrementing unique identifier
    track_id INTEGER, -- Track ID (not unique)
    track_name TEXT,
    recording_artist_id INTEGER,
    recording_artist_name TEXT,
    album_id INTEGER,
    album_title TEXT,
    release_year INTEGER,
    primary_type_name TEXT, -- Album, EP, Single
    secondary_type_name TEXT -- Live, Soundtrack, etc.
);

INSERT INTO a_first_pass (
    track_id, track_name, 
    recording_artist_id, recording_artist_name, 
    album_id, album_title, release_year, primary_type_name, secondary_type_name
)
SELECT
    rec.id AS track_id,
    rec.name AS track_name,
    ac_rec.id AS recording_artist_id,
    ac_rec.name AS recording_artist_name,
    rg.id AS album_id,
    rg.name AS album_title,
    rgm.first_release_date_year AS release_year,
    pt.name AS primary_type_name,
    st.name AS secondary_type_name
FROM
    recording rec
JOIN
    track t ON t.recording = rec.id
JOIN
    medium m ON t.medium = m.id
JOIN
    release r ON m.release = r.id
JOIN
    release_country rc ON r.id = rc.release
JOIN
    country_area ca ON rc.country = ca.area
JOIN
    release_group rg ON r.release_group = rg.id
JOIN
    artist_credit ac_rec ON rec.artist_credit = ac_rec.id
LEFT JOIN
    release_group_meta rgm ON rg.id = rgm.id
LEFT JOIN
    release_group_primary_type pt ON rg.type = pt.id
LEFT JOIN
    release_group_secondary_type_join stj ON rg.id = stj.release_group
LEFT JOIN
    release_group_secondary_type st ON stj.secondary_type = st.id
WHERE
    ca.area = 222 -- United States
    AND r.status = 1
    AND (r.comment = '' or r."comment" ilike 'explicit'); -- Standard releases only
