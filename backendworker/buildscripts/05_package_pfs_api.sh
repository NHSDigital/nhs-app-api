#!/bin/bash

set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

COMMIT_ID=$(git rev-parse --short HEAD)

docker build \
  --target=final \
  --build-arg=COMMIT_ID="$COMMIT_ID" \
  --build-arg=PROJECT_NAME=NHSOnline.Backend.PfsApi \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-backendpfsapi:${DOCKER_TAG:-latest}" \
  .

if [ -n "$BRANCH_TAG" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-backendpfsapi:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-backendpfsapi:$BRANCH_TAG"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendpfsapi:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendpfsapi:$BRANCH_TAG"
fi