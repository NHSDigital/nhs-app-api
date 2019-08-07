#!/bin/bash

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

export APP_DOCKER_TAG="${DOCKER_TAG:-latest}"
export APP_DOCKER_REGISTRY="${DOCKER_REGISTRY:-local}"

cd ops || exit 1
./docker_tests.sh