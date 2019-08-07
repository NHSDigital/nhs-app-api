#!/bin/bash

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

COMMIT_ID=$(git rev-parse --short HEAD)

docker build \
  --target=final \
  --build-arg=COMMIT_ID="$COMMIT_ID" \
  --build-arg=PROJECT_NAME=NHSOnline.Backend.ServiceJourneyRulesApi \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-backendservicejourneyrulesapi:${DOCKER_TAG:-latest}" \
  .
if [ ! -z "$BRANCH_TAG" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-backendservicejourneyrulesapi:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-backendservicejourneyrulesapi:$BRANCH_TAG"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendservicejourneyrulesapi:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendservicejourneyrulesapi:$BRANCH_TAG"
fi