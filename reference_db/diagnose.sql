---Full query
/*
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
    */

--Search for artists
select *
from artist_credit
where name = 'Classic Nouveaux'

--Search for Albums
select *
from release_group rg 
where name ilike 'Yourself or Someone Like You'

--Search for Songs by an Artist
SELECT
    rec.id AS track_id,
    rec.name AS track_name,
    ac_rec.id AS recording_artist_id,
    ca.area = 222 and r.status = 1 and (r.comment = '' or r."comment" ilike 'explicit') is_included,
    rg.id AS album_id,
    ac_rec.name AS recording_artist_name,
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
where 1=1
and ac_rec.name ilike '%december%'
--and t.name ilike '%mermaids%'
and rg.name ilike '%Picaresque%'
order by rgm.first_release_date_year
	

select *
from artist_credit ac 
where ac.name ilike '%better beatles%'

select *
from a_first_pass afp 
where recording_artist_name ilike '%december%'
and track_name ilike '%mariner%'

select *
from a_first_pass afp 
where 
1 = 1
--and recording_artist_name ilike 'An Emotional Fish'
--and track_name = 'Rain%'
and recording_artist_name ilike '%december%'
and track_name ilike '%mariner%'

select *
from a_first_pass afp 
where album_id = 106917

select *
from a_second_pass 
where 
1 = 1
--and recording_artist_name ilike 'Eric Clapton'
and recording_artist_name ilike '%december%'
and track_name ilike '%mariner%'


select *
from a_third_pass atp 
where 1=1
and recording_artist_name ilike '%december%'
and track_name ilike '%mariner%'


select * from a_ranked_tracks 
where 
1=1
and recording_artist_name ilike '%december%'
and track_name ilike '%mariner%'

update a_ranked_tracks set the_rank = 1 where track_id = 15395474;
update a_ranked_tracks set the_rank = 10 where track_id = 23408649;
--and album_title ilike '%unplugged%'

with singles_rank as (
SELECT
    track_name,
    recording_artist_id,
    MIN(release_year) AS earliest_single_year
FROM a_third_pass
WHERE primary_type_name = 'Single' and recording_artist_name = 'The Decemberists' and track_name = 'The Mariners Revenge Song'
GROUP BY track_name, recording_artist_id)
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
    CASE
                -- Albums take precedence unless a single is more than 1 year earlier
	    		when primary_type_name = 'Album' and sr.earliest_single_year is null then 1
                WHEN primary_type_name = 'Album' AND sr.earliest_single_year + 1 >= release_year THEN 2
                WHEN primary_type_name = 'Single' THEN 3
                WHEN primary_type_name = 'Album' AND secondary_type_name IN ('Soundtrack', 'Live') THEN 4
                WHEN primary_type_name = 'EP' THEN 5
                ELSE 6
            end foobar,
    ROW_NUMBER() OVER (
        PARTITION BY fp.track_name, fp.recording_artist_id
        ORDER BY
            release_year ASC, -- Earliest release year for ties
            CASE
                -- Albums take precedence unless a single is more than 1 year earlier
                when primary_type_name = 'Album' and sr.earliest_single_year is null then 1
                WHEN primary_type_name = 'Album' AND sr.earliest_single_year + 1 >= release_year THEN 2
                WHEN primary_type_name = 'Single' THEN 3
                WHEN primary_type_name = 'Album' AND secondary_type_name IN ('Soundtrack', 'Live') THEN 4
                WHEN primary_type_name = 'EP' THEN 5
                ELSE 6
            END,
            album_id ASC -- Tie-breaker
    ) AS the_rank
FROM a_third_pass fp
LEFT JOIN singles_rank sr
    ON fp.track_name = sr.track_name
    AND fp.recording_artist_id = sr.recording_artist_id
where fp.recording_artist_name = 'The Decemberists' and fp.track_name = 'The Mariners Revenge Song';