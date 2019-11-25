#!/usr/bin/env bash

set -e

echo "Commit hash: $(git rev-parse HEAD)"

IMAGE=nhsonline-backendservicejourneyrulesapi
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
CONFIGURATION_FOLDER_PATH=/app/configurations

docker pull  ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD)
docker run -e "SJR_ARGS=--validate-only" \
           -e "OUTPUT_FOLDER_PATH=$CONFIGURATION_FOLDER_PATH" \
           -e "GP_INFO_FILE_PATH=$CONFIGURATION_FOLDER_PATH/gpinfo.csv" \
           -e "CONFIGURATION_FOLDER_PATH=$CONFIGURATION_FOLDER_PATH" \
           -v $PWD/configurations/:$CONFIGURATION_FOLDER_PATH \
           ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD)