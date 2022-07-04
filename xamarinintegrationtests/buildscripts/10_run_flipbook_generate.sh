#! /usr/bin/env bash
set -ex

# Change current working directory to be the root of flipbook, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/../flipbookgeneration" || exit 1

DOCKER_ARGS=()
DOCKER_ARGS+=(--name "int_flipbook")

# Remove previous docker container
if [ "$(docker ps -a | grep int_flipbook)" ] 
 then docker rm "$(docker ps -a | grep int_flipbook | awk '{print $1}')"
fi

echo "Generating flipbook..."

docker run \
  "${DOCKER_ARGS[@]}" \
  "${DOCKER_REGISTRY:-local}/flipbook-generation:${DOCKER_TAG:-latest}" bash -c "
   FLIPBOOK_NAME=${FLIPBOOK_NAME:-flipbook_local} ./generate.sh"

docker cp int_flipbook:/src/flipbook/. flipbook

echo "Done."