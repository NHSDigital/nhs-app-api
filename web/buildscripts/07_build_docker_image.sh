#!/bin/bash

set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

docker build \
  --target=release \
  --cache-from=local/nhsonline-web-production-dependencies \
  --cache-from=local/nhsonline-web-build \
  --build-arg=COMMIT_ID="$(git rev-parse --short HEAD)" \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-web:${DOCKER_TAG:-latest}" \
  .

if [ -n "$BRANCH_TAG" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-web:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-web:$BRANCH_TAG"
  docker push "${DOCKER_REGISTRY}/nhsonline-web:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-web:$BRANCH_TAG"
fi
