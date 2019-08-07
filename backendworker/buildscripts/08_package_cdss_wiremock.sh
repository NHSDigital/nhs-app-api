#!/bin/bash

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

docker build \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-backendcdsswiremock:${DOCKER_TAG:-latest}" \
  NHSOnline.Backend.PfsApi/ClinicalDecisionSupport/cdss-wiremock

if [ ! -z "$BRANCH_TAG" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:$BRANCH_TAG"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:$BRANCH_TAG"
fi