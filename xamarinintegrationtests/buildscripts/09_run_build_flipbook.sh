#! /usr/bin/env bash
set -ex

# Change current working directory to be the root of flipbook regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/../flipbookgeneration" || exit 1

chmod a+x generate.sh

docker build \
    --tag="${DOCKER_REGISTRY:-local}/flipbook-generation:${DOCKER_TAG:-latest}" \
  .