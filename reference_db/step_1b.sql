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

UPDATE a_third_pass SET track_name = track_name || ' (Unplugged)' WHERE (album_id = 21187 OR album_id = 22184);
