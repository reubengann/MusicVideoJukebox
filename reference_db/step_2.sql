CREATE TABLE a_ranked_tracks (
    track_id INTEGER,
    track_name TEXT,
    recording_artist_id INTEGER,
    recording_artist_name TEXT,
    album_id INTEGER,
    album_title TEXT,
    release_year INTEGER,
    primary_type_name TEXT,
    secondary_type_name TEXT,
    the_rank INTEGER
);

CREATE TEMP TABLE singles_rank AS
SELECT
    track_name,
    recording_artist_id,
    MIN(release_year) AS earliest_single_year
FROM a_third_pass
WHERE primary_type_name = 'Single'
GROUP BY track_name, recording_artist_id;

INSERT INTO a_ranked_tracks (
    track_id,
    track_name,
    recording_artist_id,
    recording_artist_name,
    album_id,
    album_title,
    release_year,
    primary_type_name,
    secondary_type_name,
    the_rank
)
SELECT
    track_id,
    fp.track_name,
    fp.recording_artist_id,
    recording_artist_name,
    album_id,
    album_title,
    release_year,
    primary_type_name,
    secondary_type_name,
    ROW_NUMBER() OVER (
        PARTITION BY fp.track_name, fp.recording_artist_id
        ORDER BY
            CASE
                -- Albums take precedence unless a single is more than 1 year earlier
                WHEN primary_type_name = 'Album' AND secondary_type_name IN ('Soundtrack', 'Live') THEN 4
                when primary_type_name = 'Album' and sr.earliest_single_year is null then 1
                WHEN primary_type_name = 'Album' AND sr.earliest_single_year + 1 >= release_year THEN 2
                WHEN primary_type_name = 'Single' THEN 3
                WHEN primary_type_name = 'EP' THEN 5
                ELSE 6
            END,
            release_year ASC, -- Earliest release year for ties
            album_id ASC -- Tie-breaker
    ) AS the_rank
FROM a_third_pass fp
LEFT JOIN singles_rank sr
    ON fp.track_name = sr.track_name
    AND fp.recording_artist_id = sr.recording_artist_id;
--    where 1=1
--and track_name = 'The Touch' and recording_artist_name = 'Stan Bush & Barrage'-- album should be The Transformers
--and track_name = 'Danger Zone' and recording_artist_name = 'Kenny Loggins' -- should be 1986 and Top Gun
--and recording_artist_name = 'Radiohead' and track_name = '15 Step' -- should be 2007 In Rainbows
--and recording_artist_name = 'AC/DC' and track_name = 'Dirty Deeds Done Dirt Cheap' -- should be in same titled album, 1976
--and recording_artist_name = 'Madness' and track_name = 'Grey Day' -- should exist on eponymous album from 1983
--and track_name = 'Under Pressure' and recording_artist_name = 'Queen & David Bowie' order by release_year -- should be Hot space from 1982, although the single was released in 1981
--and recording_artist_name in ('LL Cool J', 'L.L. Cool J') and track_name = 'I Need Love' -- Bigger and Deffer