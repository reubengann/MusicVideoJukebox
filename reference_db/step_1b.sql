-- Consolidate all tracks with the same name (except for capitalization) together
-- Eliminates song titles like Far From Me and Far from Me.
CREATE TEMP TABLE normalized_tracks AS
    SELECT
        recording_artist_id,
        lower(trim(regexp_replace(
            regexp_replace(track_name, '[^\w\s]', '', 'g'), -- Remove non-alphanumeric characters
            '\s+', ' ', 'g' -- Normalize spaces
        ))) AS normalized_track_name,
        track_name,
        CASE 
            WHEN INITCAP(track_name) = track_name THEN 1 -- Properly capitalized
            ELSE 2 -- Not properly capitalized
        END AS capitalization_score
    FROM a_second_pass;



CREATE TEMP TABLE best_track_names AS
SELECT
    normalized_track_name,
    track_name AS best_spelling,
    recording_artist_id
FROM (
    SELECT
        normalized_track_name,
        track_name,
        capitalization_score,
        recording_artist_id,
        ROW_NUMBER() OVER (
            PARTITION BY normalized_track_name, recording_artist_id
            ORDER BY capitalization_score ASC, -- Prioritize proper capitalization
                     track_name ASC -- Lexically first as a fallback
        ) AS rank
    FROM normalized_tracks
) ranked
WHERE rank = 1;

CREATE TABLE IF NOT EXISTS a_third_pass AS
SELECT
    asp.track_id,
    bnt.best_spelling AS track_name, -- Replace with the best spelling
    asp.recording_artist_id,
    asp.recording_artist_name,
    asp.album_id,
    asp.album_title,
    asp.release_year,
    asp.primary_type_name,
    asp.secondary_type_name
FROM a_second_pass asp
LEFT JOIN best_track_names bnt
    ON lower(trim(regexp_replace(
        regexp_replace(asp.track_name, '[^\w\s]', '', 'g'),
        '\s+', ' ', 'g'
    ))) = bnt.normalized_track_name
    and asp.recording_artist_id = bnt.recording_artist_id;

UPDATE a_third_pass SET track_name = track_name || ' (Unplugged)' WHERE album_id in (21187, 22184, 43144);
UPDATE a_third_pass SET track_name = track_name || ' (Hell Freezes Over)' WHERE album_id = 20427;
UPDATE a_third_pass SET track_name = track_name || ' (West 54th)' WHERE album_id = 106917;
UPDATE a_third_pass SET track_name = track_name || ' (Brixton)' WHERE album_id = 352130;
UPDATE a_third_pass SET track_name = track_name || ' (Monserrat)' WHERE album_id = 924914;
UPDATE a_third_pass SET track_name = track_name || ' (The Dance)' WHERE album_id = 32332;
UPDATE a_third_pass SET track_name = track_name || ' (Austin City Limits)', release_year = 1981 WHERE album_id = 1062263;
UPDATE a_third_pass SET track_name = track_name || ' (Unledded)' WHERE album_id = 567705;
UPDATE a_third_pass SET track_name = track_name || ' (A Black and White Night)' WHERE album_id = 96095;
UPDATE a_third_pass SET track_name = track_name || ' (In Central Park)' WHERE album_id = 195560;
UPDATE a_third_pass SET track_name = track_name || ' (Stop Making Sense)' WHERE album_id = 979654;
