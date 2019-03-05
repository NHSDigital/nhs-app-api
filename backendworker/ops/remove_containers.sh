#!/usr/bin/env bash

APIS=(pfsapi cidapi)
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}

for api in "${APIS[@]}"
do
    docker rmi ${DOCKER_REGISTRY}/nhsonline-backend$api:$(git rev-parse HEAD)
done