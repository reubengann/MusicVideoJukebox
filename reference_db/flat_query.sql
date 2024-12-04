select distinct on (rec.name, ac.id)
    rec.id track_id, 
    rec."name" track_name,
    ac.id AS artist_id,
    ac.name as artist_name,
    rg.id as release_group_id,
    rg.name as release_name,
    pt.id as primary_type_id,
    pt.name as primary_type_name,
    st.id as secondary_type_id,
    st.name as secondary_type_name,
    rgm.first_release_date_year release_year
FROM
    release_group rg
JOIN
    artist_credit ac ON rg.artist_credit = ac.id
LEFT JOIN
    release_group_primary_type pt ON rg.type = pt.id
LEFT JOIN
    release_group_secondary_type_join stj ON rg.id = stj.release_group
LEFT JOIN
    release_group_secondary_type st ON stj.secondary_type = st.id
join artist a on a.id = ac.id
JOIN
    release_group_meta rgm on rg.id = rgm.id
join release r on r.release_group = rg.id
join release_country rc on r.id = rc."release"
join country_area ca on rc.country = ca.area
join recording rec on rec.artist_credit = ac.id
where pt.name in ('Album', 'EP', 'Single')
AND (st.name IS NULL OR st.name NOT IN ('Bootleg', 'Compilation'))
and ca.area = 222 --united states
order by 
	rec.name, 
	ac.id, 
	CASE pt.name 
        WHEN 'Album' THEN 1
        WHEN 'EP' THEN 2
        WHEN 'Single' THEN 3
        ELSE 4
    END, 
    rgm.first_release_date_year ASC,
    rg.id ASC -- Final tie-breaker
