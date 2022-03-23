#! /usr/bin/env bash

set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

DOCKER_COMPOSE_ARGS=(-f docker-compose.yml -f docker/inttests/docker-compose.yml)

if on_build_agent; then
  DOCKER_COMPOSE_ARGS+=(-f docker/inttests/docker-compose.host-docker-internal.yml)
fi

docker-compose "${DOCKER_COMPOSE_ARGS[@]}" down
