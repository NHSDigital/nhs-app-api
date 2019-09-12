#!/usr/bin/env bash

APIS=(backendpfsapi backendcidapi backendservicejourneyrulesapi backendcdsswiremock clientloggerapi)
DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}

for api in "${APIS[@]}"
do
    docker rmi ${DOCKER_REGISTRY}/nhsonline-$api:$(git rev-parse HEAD)
done