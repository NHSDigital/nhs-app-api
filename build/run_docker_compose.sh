#!/bin/bash

REPO_ROOT=$(dirname "$(dirname "${BASH_SOURCE[0]}")")

FILES=()
for file in "$@"; do
  FILES+=(-f "$file")
done
for file in $(env | grep _DOCKER_PORTS | sed "s#^.*_DOCKER_PORTS=#${REPO_ROOT//\\/\/}/docker/#"); do
  FILES+=(-f "$file")
done

echo "Docker compose files: ${FILES[*]}"

if [ -z "$NO_PULL" ]; then
  for IMAGE_TO_PULL in $(docker-compose "${FILES[@]}" config | grep image | sed -e 's/^[[:space:]]*image: //' | sort -u); do
    case "$IMAGE_TO_PULL" in
      local/*) echo "Skipping pull on local image $IMAGE_TO_PULL";;
      *) docker pull "$IMAGE_TO_PULL";;
    esac
  done
fi

docker-compose "${FILES[@]}" down
docker-compose "${FILES[@]}" up
docker-compose "${FILES[@]}" down
