#!/usr/bin/env bash

set -e

echo "Commit hash: $(git rev-parse HEAD)"
echo "Commit hash: $(git rev-parse HEAD)" > version.txt

IMAGE=nhsonline-backend-sjr-validation
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
OUTPUT_FOLDER_PATH=/src/NHSOnline.Backend.ServiceJourneyRulesApi/configurations/output/

docker build --pull --build-arg SJR_ARGS=--validate-only -t ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD) -f NHSOnline.Backend.ServiceJourneyRulesApi/Dockerfile .
docker run -e "OUTPUT_FOLDER_PATH=$OUTPUT_FOLDER_PATH" -v $PWD/configurations/:$OUTPUT_FOLDER_PATH ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD)