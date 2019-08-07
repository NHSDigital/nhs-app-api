#!/bin/bash

REPO_ROOT=$(dirname `dirname ${BASH_SOURCE[0]}`)

PORT_FILES=$(env | grep _DOCKER_PORTS | sed "s#^.*_DOCKER_PORTS=#${REPO_ROOT//\\/\/}/docker/#")
FILES=$(echo $* $PORT_FILES | sed "s/ / -f /g")
echo "Docker compose files: $FILES"

if [ -z "$NO_PULL" ]; then
    DOCKER_IMAGES=$(docker-compose -f $FILES config | grep image | sed -e 's/^[[:space:]]*image: //' | sort -u)

    # Don't try to pull local images
    DOCKER_IMAGES_TO_PULL=$(echo $DOCKER_IMAGES | sed -e 's#local/[^[:space:]]\+##g' -e 's/ \+/ /')
    for IMAGE_TO_PULL in $DOCKER_IMAGES_TO_PULL; do
        docker pull $IMAGE_TO_PULL
    done
fi

docker-compose -f $FILES down
docker-compose -f $FILES up
docker-compose -f $FILES down
