#! /usr/bin/env bash

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source buildscripts/lib/set_env.sh

# shellcheck source=lib/generate_host_settings.sh
source buildscripts/lib/generate_host_settings.sh

DOCKER_COMPOSE_FILES_ARGS=()
for file in "$@"; do
  DOCKER_COMPOSE_FILES_ARGS+=(-f "$file")
done
for file in $(env | grep _DOCKER_PORTS | sed "s#^.*_DOCKER_PORTS=#docker/#"); do
  DOCKER_COMPOSE_FILES_ARGS+=(-f "$file")
done

echo "Docker compose files: ${DOCKER_COMPOSE_FILES_ARGS[*]}"

if [ -z "$NO_PULL" ]; then
  for IMAGE_TO_PULL in $(docker-compose "${DOCKER_COMPOSE_FILES_ARGS[@]}" config | grep image | sed -e 's/^[[:space:]]*image: //' | sort -u); do
    case "$IMAGE_TO_PULL" in
      local/*) echo "Skipping pull on local image $IMAGE_TO_PULL";;
      *) docker pull "$IMAGE_TO_PULL";;
    esac
  done
fi

generate_host_settings

docker-compose "${DOCKER_COMPOSE_FILES_ARGS[@]}" down
docker-compose "${DOCKER_COMPOSE_FILES_ARGS[@]}" up
docker-compose "${DOCKER_COMPOSE_FILES_ARGS[@]}" down
