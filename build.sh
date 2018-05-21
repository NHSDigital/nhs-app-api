#!/usr/bin/env bash

set -e

if [ ! -x $(command -v docker) ];
then
  echo "Docker is missing.";
  exit 1
fi

if [ -z "${DOCKER_REGISTRY}" ];
then
  echo "Missing DOCKER_REGISTRY variable."
  exit 1
fi

docker build . -t ${DOCKER_REGISTRY}/nhsonline-backendworker:$(git rev-parse HEAD) -f NHSOnline.Backend.Worker/Dockerfile
docker tag ${DOCKER_REGISTRY}/nhsonline-backendworker:$(git rev-parse HEAD) ${DOCKER_REGISTRY}/nhsonline-backendworker:latest
