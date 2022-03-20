#!/bin/sh

docker stop wasted-db wasted-db-test
docker rm wasted-db wasted-db-test
docker image rm wasted-db:latest
docker build -t  wasted-db ~/Repos/wastedapi/database/
docker run -d --name wasted-db -p 54321:5432 wasted-db
docker run -d --name wasted-db-test -p 54323:5432 wasted-db
