#!/usr/bin/env bash

set -e
echo "Commit hash: $(git rev-parse HEAD)"
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
CURRENT_TAG=$(git name-rev --tags --name-only $CURRENT_BRANCH | sed 's/\^.*//g')
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
APIS=(backendpfsapi backendcidapi backendservicejourneyrulesapi backendcdsswiremock backendusersapi clientloggerapi)

echo "Current tag: $CURRENT_TAG"

if [ "$CURRENT_TAG" != "undefined" ]; then
  REGISTRY_TAG=$CURRENT_TAG
else
  REGISTRY_TAG=latest
fi
echo "Setting registry tag: $REGISTRY_TAG"

for api in "${APIS[@]}"
do
    docker pull ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD)
    docker tag ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD) ${DOCKER_REGISTRY}/nhsonline-$api:$REGISTRY_TAG

    docker push ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD)
    docker push ${DOCKER_REGISTRY}/nhsonline-$api:$REGISTRY_TAG
    #docker rmi ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD)
    docker rmi ${DOCKER_REGISTRY}/nhsonline-$api:$REGISTRY_TAG
done