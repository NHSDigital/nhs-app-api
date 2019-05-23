#!/usr/bin/env bash

set -e

echo "Commit hash: $(git rev-parse HEAD)"
echo "Commit hash: $(git rev-parse HEAD)" > version.txt

APIS=(PfsApi CidApi ServiceJourneyRulesApi ClinicalDecisionSupportApi)
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}

for api in "${APIS[@]}"
do
  docker build --pull --build-arg COMMIT_ID=$(git rev-parse HEAD) -t ${DOCKER_REGISTRY}/nhsonline-backend$(echo "$api" | tr '[:upper:]' '[:lower:]'):$(git rev-parse HEAD) -f NHSOnline.Backend.$api/Dockerfile .
done

# build CDSS Wiremock image
cd NHSOnline.Backend.ClinicalDecisionSupportApi/cdss-wiremock
docker build --pull --build-arg COMMIT_ID=$(git rev-parse HEAD) -t ${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:$(git rev-parse HEAD) .