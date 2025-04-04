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

--Get everything that had a US release
CREATE TEMP TABLE us_albums AS
SELECT DISTINCT rg.id AS album_id
    FROM release r
    JOIN medium m ON r.id = m.release
    JOIN track t ON m.id = t.medium
    JOIN release_country rc ON r.id = rc.release
    JOIN country_area ca ON rc.country = ca.area
    JOIN release_group rg ON r.release_group = rg.id
    WHERE ca.area = 222; -- United States

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
JOIN us_albums ua ON rg.id = ua.album_id
WHERE
    r.status = 1 --Official releases
    AND (r.comment = '' or r."comment" ilike 'explicit'); -- Standard releases only

-- Individual corrections
UPDATE a_first_pass SET track_name = 'World (The Price of Love)' where recording_artist_id = 846 and track_name = 'World';
UPDATE a_first_pass SET track_name = REGEXP_REPLACE(track_name, '^92°(.*)', '92°F\1') where recording_artist_id = 2127 and track_name like '92°%' AND track_name NOT LIKE '92°F%';
UPDATE a_first_pass SET track_name = 'Pressure Us' WHERE track_id = 629988; --This data is just wrong
UPDATE a_first_pass SET track_name = REGEXP_REPLACE(
    track_name,
    '\s-\s(.*)$', -- Matches " - " and captures everything after it
    ' (\1)',      -- Wrap the captured content in parentheses
    'gi'          -- Case-insensitive
)
WHERE track_name ~* '\s-\s.*(remastered|demo|version|edit|mix).*';
UPDATE a_first_pass SET track_name = REGEXP_REPLACE(track_name, '^"(.*)"$', '\1', 'g') WHERE track_name ~ '^".*"$'; --tracks where the name is in quotes

--Albums to add explicitly
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
WHERE rg.id in (
    165100 --An Emotional Fish	Junk Puppets
    , 1051317 --Balaam and the Angel, The Greatest Story Ever Told
    , 212130 --	Blancmange, Believe You Me
    , 149223--	China Drum	Goosefair
    , 717936--	Classix Nouveaux	Night People
    , 2592000 --	Descendents	Fartathon (Live in St. Louis 1987)
    , 968630 --	Gumball	Pandemonic
    , 207055 --	Gumball	Revolution on Ice
    , 559460 --	The House of Love	You Don't Understand
    , 547869--	Huxton Creepers	12 Days to Paris
    , 134541 --	James	Stutter
    , 270184 --	Lifter	Turn On Tune Out
    , 530234 --	Lifter	huH, Volume 18
    , 233274 --	Lime Spiders	Volatile
    , 37193 --	Luscious Jackson	Scoop This Too!
    , 62057 --	Marvelous 3	Hey! Album
    , 38840 --	Matchbox 20	Yourself or Someone Like You
    , 1704544 --	Mood Six	The Difference Is ......
    , 718729 --	Mood Six	I Saw the Light
    , 124719 --	Platinum Blonde	Alien Shores
    , 202364 --	Rose Tattoo	Scarred for Life
    , 644265 --	The Arrows	Lines Are Open
    , 42524 --	The Charlatans	The Charlatans
    , 1442173 --	The Cherry Bombz	Coming Down Slow
    , 353421 --	The House of Love	Babe Rainbow
    , 155444 --	Latin Quarter	Modern Times
    , 334764 --	The Long Ryders	State of Our Union
    , 255822 --	The Other Ones	The Other Ones
    , 871979 --	The Rumble	American Heart & Soul
    , 54428 --	The Wolfgang Press	Lonely Is an Eyesore
    , 320312 --	Wool	Budspawn
    , 1284405 --	Zerra One	The Domino Effect
    , 105138 --	soulDecision	No One Does It Better
    , 924914 --	Eric Clapton	Music for Monserrat (Benefit Concert)
    , 742213 --	Paul Heaton	The Cross Eyed Rambler
    , 331825 --	Linkin Park	Meteora
    , 621956 -- 30 Seconds to Marks A Beautiful Lie
    , 3186287-- Alan Walker, Walkerverse Pt. I & II
    , 2943010 -- Anitta Versions of Me
    , 2760803 --	Baby Keem & Kendrick Lamar	The Melodic Blue
    , 1973281 --	Banda Los Recoditos	Sueño XXX
    , 3860698 --	Beach Bunny	Clueless
    , 1791853 --	Cashmere Cat feat. Selena Gomez & Tory Lanez	9
    );

--standardize artists
UPDATE a_first_pass SET recording_artist_id = 1075755, recording_artist_name = 'Huey Lewis & the News' WHERE recording_artist_id = 936866;
UPDATE a_first_pass SET recording_artist_id = 1424570, recording_artist_name = 'Joan Jett & the Blackhearts' WHERE recording_artist_id = 10699;
UPDATE a_first_pass SET recording_artist_id = 22656, recording_artist_name = 'LL Cool J' WHERE recording_artist_id = 1088962;
UPDATE a_first_pass SET recording_artist_id = 509, recording_artist_name = 'Goo Goo Dolls' WHERE recording_artist_id = 940807;
UPDATE a_first_pass SET recording_artist_id = 803, recording_artist_name = 'The Pretenders' WHERE recording_artist_id = 855259;
UPDATE a_first_pass SET recording_artist_id = 53815, recording_artist_name = 'Tony! Toni! Toné!' WHERE recording_artist_id = 846666;
UPDATE a_first_pass SET recording_artist_id = 96, recording_artist_name = 'Bangles' WHERE recording_artist_id = 843692;
UPDATE a_first_pass SET recording_artist_id = 11650, recording_artist_name = 'The Smashing Pumpkins' WHERE recording_artist_id = 933469;

--This is annoying
UPDATE a_first_pass SET recording_artist_name = 'Weird Al Yankovic' WHERE recording_artist_id = 863324;

DELETE from a_first_pass where album_id = 38468; -- Best of Nick Cave came out almost exactly with other albums and fucks things up
DELETE from a_first_pass where recording_artist_id = 591414; -- The Better Beatles
DELETE from a_first_pass where track_id = 21554716; -- Incorrectly says that Australia was on Chutes Too Narrow
