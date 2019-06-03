#!/usr/bin/env bash

set -e

echo "Commit hash: $(git rev-parse HEAD)"
echo "Commit hash: $(git rev-parse HEAD)" > version.txt

IMAGE=nhsonline-backendservicejourneyrulesapi
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
OUTPUT_FOLDER_PATH=/src/NHSOnline.Backend.ServiceJourneyRulesApi/configurations/output/

docker pull  ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD)
docker run -e "SJR_ARGS=--validate-only" -e "OUTPUT_FOLDER_PATH=$OUTPUT_FOLDER_PATH" -v $PWD/configurations/:$OUTPUT_FOLDER_PATH ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD)