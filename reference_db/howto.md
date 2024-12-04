Set up the MusicBrainz database to run locally

```bash
git clone https://github.com/metabrainz/musicbrainz-docker.git
cd musicbrainz-docker
```

Modify the docker-compose so that the `db` service has

```yaml
ports: 
    - "5432:5432"
```

```bash
docker-compose build
docker-compose run --rm musicbrainz createdb.sh -fetch
docker-compose up -d
docker-compose exec indexer python -m sir reindex
```

Now, from the `reference_db` folder, run

```bash
python make_reference_db.py
```

Store this file in `MusicVideoJukeBox` folder.
