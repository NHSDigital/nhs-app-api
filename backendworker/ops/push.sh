#!/usr/bin/env bash

APIS=(backendpfsapi backendcidapi backendservicejourneyrulesapi backendcdsswiremock backendusersapi backendmessagesapi clientloggerapi)
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}

for api in "${APIS[@]}"
do
  docker tag ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD) ${DOCKER_REGISTRY}/nhsonline-$api:$BRANCH_TAG
  docker push ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD)
  docker push ${DOCKER_REGISTRY}/nhsonline-$api:$BRANCH_TAG

  if [ "$BRANCH_TAG" == "develop" ]; then
    docker tag ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD) ${DOCKER_REGISTRY}/nhsonline-$api:latest
    docker push ${DOCKER_REGISTRY}/nhsonline-$api:latest
    docker rmi ${DOCKER_REGISTRY}/nhsonline-$api:latest
  fi

  docker rmi ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD)
  docker rmi ${DOCKER_REGISTRY}/nhsonline-$api:$BRANCH_TAG
done