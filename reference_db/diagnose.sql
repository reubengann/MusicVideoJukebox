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
    ac_rec.name AS recording_artist_name,
    rg.id AS album_id,
    rg.name AS album_title,
    rgm.first_release_date_year AS release_year,
    pt.name AS primary_type_name,
    st.name AS secondary_type_name,
    ca.area = 222 is_us
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
where ac_rec.name = 'An Emotional Fish'
and t.name ilike 'Rain%'

select *
from a_first_pass afp 
where afp.track_id = 629988

select *
from a_first_pass afp 
where recording_artist_name ilike 'An Emotional Fish'
and track_name = 'Rain%'
--and album_title ilike '%unplugged%'

select *
from a_second_pass 
where recording_artist_name ilike 'Nirvana'
and album_title ilike '%unplugged%'


select *
from a_third_pass atp 
where recording_artist_name ilike 'Nirvana'
and album_title ilike '%unplugged%'


select * from a_ranked_tracks 
where recording_artist_name ilike 'Nirvana'
and album_title ilike '%unplugged%'

update a_ranked_tracks set the_rank = 1 where track_id = 15395474;
update a_ranked_tracks set the_rank = 10 where track_id = 23408649;
--and album_title ilike '%unplugged%'