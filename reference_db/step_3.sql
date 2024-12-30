CREATE TABLE musicbrainz.a_rank_1_tracks AS
SELECT *
FROM musicbrainz.a_ranked_tracks
WHERE the_rank = 1;