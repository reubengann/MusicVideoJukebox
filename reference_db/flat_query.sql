with cte as (
select rec.id AS track_id,
        rec.name AS track_name,
        lower(trim(translate(regexp_replace(
        CASE
            WHEN rec.name ~ '^\(.*\)' THEN rec.name -- Keep full name if it starts with parentheses
        	ELSE SPLIT_PART(rec.name, '(', 1) -- Otherwise, truncate at first opening parenthesis
        end,
        '\([^)]*\)$', '', 'g'),
        '’', ''''))) AS normalized_track_name,
        ac.id AS artist_id,
        ac.name AS artist_name,
        rg.id AS album_id,
        rg.name AS album_title,
        pt.id as primary_type_id,
        pt.name AS primary_type_name,
        st.id as secondary_type_id,
        st.name AS secondary_type_name,
        rgm.first_release_date_year release_year,
        ROW_NUMBER() OVER (
            PARTITION BY 
                ac.id, 
                lower(trim(translate(regexp_replace(
		        CASE
		            WHEN rec.name ~ '^\(.*\)' THEN rec.name -- Keep full name if it starts with parentheses
		        	ELSE SPLIT_PART(rec.name, '(', 1) -- Otherwise, truncate at first opening parenthesis
		        end,
		        '\([^)]*\)$', '', 'g'),
		        '’', '''')))
            ORDER BY 
                CASE 
                    WHEN pt.name = 'Album' AND (st.name IS NULL OR st.name NOT IN ('Live', 'Compilation')) THEN 1 -- Studio albums
                    WHEN pt.name = 'Album' AND st.name in ('Live', 'Soundtrack') THEN 2 -- Live albums
                    WHEN pt.name = 'EP' THEN 3 -- EPs
                    WHEN pt.name = 'Single' THEN 4 -- Singles
                    ELSE 5 -- Everything else
                END,
                rgm.first_release_date_year ASC, -- Earliest release date
                rg.id ASC -- Tie-breaker
        ) AS rank
FROM
    recording rec
JOIN
    track t ON t.recording = rec.id
JOIN
    medium m ON t.medium = m.id
JOIN
    release r ON m.release = r.id
JOIN
    release_country rc ON r.id = rc.release -- Filter by country
JOIN
    country_area ca ON rc.country = ca.area
JOIN
    release_group rg ON r.release_group = rg.id
JOIN
    artist_credit ac ON rg.artist_credit = ac.id
LEFT JOIN
    release_group_primary_type pt ON rg.type = pt.id
LEFT JOIN
    release_group_secondary_type_join stj ON rg.id = stj.release_group
LEFT JOIN
    release_group_secondary_type st ON stj.secondary_type = st.id
JOIN release_group_meta rgm ON rg.id = rgm.id
WHERE
    pt.name IN ('Album', 'EP', 'Single') -- Include only relevant primary types
    AND (st.name IS NULL OR st.name NOT IN ('Bootleg', 'Compilation', 'Remix', 'Demo', 'Interview')) -- Exclude irrelevant secondary types
    and r.status = 1
    and ca.area = 222 --united states
    and ac.id != 1
    --and ac.name = 'Radiohead'
    --and ac.name = 'AC/DC'
    --and ac.name = 'Coldplay'
    --and ac.name = 'The Beatles'
    --and ac.name = 'The Rolling Stones'
    --and rec.id = 4210
    and r.comment = '' -- skip deluxe boxed sets, etc
)
select distinct on (track_id) 
track_id, track_name, artist_id, artist_name, album_id, album_title, primary_type_id, primary_type_name, secondary_type_id, secondary_type_name, release_year
from cte where rank = 1
order by track_id, release_year