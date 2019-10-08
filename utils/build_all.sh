#!/bin/sh
STARTEDAT=$(date)
docker ps -a | awk '{print $1}' | grep -v CONTAINER | xargs docker rm -f
docker volume list | awk '{print $2}' | grep -v VOLUME | xargs docker volume rm -f
docker pull nhsapp.azurecr.io/nhsonline-redis-data:latest
cd bddtests
docker-compose build
cd ../backendworker
docker-compose -f docker-compose.servicejourneyrules.yml build
docker volume ls | grep servicejourneyrules | awk '{print $2}' | xargs docker volume rm
docker-compose build
cd ../web
docker-compose build
cd ..
echo Started:  $STARTEDAT
echo Finished: $(date)
