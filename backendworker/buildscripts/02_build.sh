#!/bin/bash

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# Cleanup old containers
OLD_CONTAINERS=$(docker images | grep nhsonline-backend | awk '{print $3}')
[ -z "$OLD_CONTAINERS"] || docker rmi -f $OLD_CONTAINERS

COMMIT_ID=$(git rev-parse --short HEAD)

docker build \
  --memory="1.8g" \
  --target=built \
  --build-arg=COMMIT_ID="$COMMIT_ID" \
  --tag "local/backend-build:$COMMIT_ID" \
  .