#!/usr/bin/env bash

APIS=(pfsapi cidapi servicejourneyrulesapi clinicaldecisionsupportapi cdsswiremock)
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}

for api in "${APIS[@]}"
do
  docker tag ${DOCKER_REGISTRY}/nhsonline-backend$api:$(git rev-parse HEAD) ${DOCKER_REGISTRY}/nhsonline-backend$api:$BRANCH_TAG
  docker push ${DOCKER_REGISTRY}/nhsonline-backend$api:$(git rev-parse HEAD)
  docker push ${DOCKER_REGISTRY}/nhsonline-backend$api:$BRANCH_TAG
  docker rmi ${DOCKER_REGISTRY}/nhsonline-backend$api:$(git rev-parse HEAD)
  docker rmi ${DOCKER_REGISTRY}/nhsonline-backend$api:$BRANCH_TAG
done