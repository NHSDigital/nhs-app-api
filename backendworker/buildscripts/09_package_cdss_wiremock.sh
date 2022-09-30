#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

pull_docker_image "wiremock/wiremock"

docker build \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-backendcdsswiremock:${DOCKER_TAG:-latest}" \
  NHSOnline.Backend.PfsApi/ClinicalDecisionSupport/cdss-wiremock || die "Failed to build ${DOCKER_REGISTRY:-local}/nhsonline-backendcdsswiremock:${DOCKER_TAG:-latest}"

if [ -n "${BRANCH_TAG}" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${BRANCH_TAG}" || die "Failed to tag ${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${BRANCH_TAG}"
  push_docker_image "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${DOCKER_TAG}"
  push_docker_image "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${BRANCH_TAG}"
  if [ -n "${DOCKER_REGISTRY_RELEASE}" ]
  then
    docker tag "${DOCKER_REGISTRY}/nhsonline-backendcdsswiremock:${DOCKER_TAG}" "${DOCKER_REGISTRY_RELEASE}/nhsonline-backendcdsswiremock:${BRANCH_TAG}"
    push_docker_image "${DOCKER_REGISTRY_RELEASE}/nhsonline-backendcdsswiremock:${BRANCH_TAG}"
  fi
fi