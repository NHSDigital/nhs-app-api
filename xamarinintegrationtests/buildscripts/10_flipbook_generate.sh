#! /usr/bin/env bash
set -e

# Change current working directory to be the root of flipbook, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/../flipbookgeneration" || exit 1

DOCKER_ARGS=()
DOCKER_ARGS+=(--name "int_flipbook")

echo "Generating flipbook..."

docker run \
  "${DOCKER_ARGS[@]}" \
  "${DOCKER_REGISTRY:-local}/flipbook-generation:${DOCKER_TAG:-latest}" bash -c "
   ./generate.sh"

docker cp int_flipbook:/src/flipbook/. flipbook
echo "Done."