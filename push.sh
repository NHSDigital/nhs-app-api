#!/usr/bin/env bash

set -e

if [ ! -x $(command -v docker) ];
then
  echo "Docker is missing.";
  exit 1
fi

docker push ${DOCKER_REGISTRY}/nhsonline-backendworker:$(git rev-parse HEAD)
docker push ${DOCKER_REGISTRY}/nhsonline-backendworker:latest
docker push ${DOCKER_REGISTRY}/nhsonline-stubs:$(git rev-parse HEAD)
docker push ${DOCKER_REGISTRY}/nhsonline-stubs:latest
