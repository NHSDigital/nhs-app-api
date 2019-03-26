#!/usr/bin/env bash

set -e

echo "Commit hash: $(git rev-parse HEAD)"
echo "Commit hash: $(git rev-parse HEAD)" > version.txt

IMAGE=nhsonline-backend-sjr-validation
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}

docker build --pull --build-arg SJR_ARGS=--validate-only -t ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD) -f NHSOnline.Backend.ServiceJourneyRulesApi/Dockerfile .
docker run ${DOCKER_REGISTRY}/$IMAGE:$(git rev-parse HEAD)