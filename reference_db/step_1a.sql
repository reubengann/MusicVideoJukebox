-- Create a_second_pass table with explicit column selection
CREATE TABLE a_second_pass AS
WITH normalized_tracks AS (
    SELECT 
        afp.track_id,
        afp.track_name,
        afp.recording_artist_id,
        afp.recording_artist_name,
        afp.album_id,
        afp.album_title,
        afp.release_year,
        afp.primary_type_name,
        afp.secondary_type_name,
        lower(trim(regexp_replace(
            afp.track_name,
            '\((?!.*\().*(edit|mix|version|instrumental|live).*\)$', -- Match only the last set of parentheses
            '',
            'gi'
        ))) AS base_name,
        CASE
            WHEN afp.track_name ~* '\((?!.*\().*(edit|mix|version|instrumental|live).*\)$' THEN 'Remove'
            ELSE 'Keep'
        END AS action
    FROM a_first_pass afp
),
tracks_to_keep AS (
    SELECT DISTINCT
        nt.track_id,
        nt.track_name,
        nt.recording_artist_id,
        nt.recording_artist_name,
        nt.album_id,
        nt.album_title,
        nt.release_year,
        nt.primary_type_name,
        nt.secondary_type_name
    FROM normalized_tracks nt
    WHERE NOT EXISTS (
        SELECT 1
        FROM normalized_tracks nt_conflict
        WHERE nt_conflict.base_name = nt.base_name
          AND nt_conflict.recording_artist_id = nt.recording_artist_id
          AND nt_conflict.action = 'Keep'
          AND nt.action = 'Remove'
    )
)
SELECT 
    track_id,
    track_name,
    recording_artist_id,
    recording_artist_name,
    album_id,
    album_title,
    release_year,
    primary_type_name,
    secondary_type_name
FROM tracks_to_keep;
