#!/bin/bash

set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# Cleanup old containers
OLD_CONTAINERS=$(docker images | grep nhsonline-web | grep -v dependencies | grep -v nhsonline-web-base | awk '{print $3}')
[ -z "$OLD_CONTAINERS" ] || docker rmi -f $OLD_CONTAINERS || true

docker build \
  --target=build \
  --cache-from=local/nhsonline-web-build-dependencies \
  --tag local/nhsonline-web-build \
  --build-arg COMMIT_ID="$(git rev-parse --short HEAD)" \
  --build-arg APP_VERSION_TAG="$BRANCH_TAG" \
  .
