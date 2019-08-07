#!/bin/bash

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

COMMIT_ID=$(git rev-parse --short HEAD)

docker build \
  --target=validated \
  --build-arg="SJR_IMAGE=${DOCKER_REGISTRY:-local}/nhsonline-backendservicejourneyrulesapi:${DOCKER_TAG:-latest}" \
  configurations