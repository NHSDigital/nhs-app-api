#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

HAWKEYE_IMAGE="${DOCKER_REGISTRY:-nhsapp.azurecr.io}/security-hawkeye:1.02"
HAWKEYE_CONTAINER_NAME=nhsonline-web-hawkeye
WORKING_DIR=$(pwd)
DOCKER_ROOT="/"

if [[ $(uname -s) =~ ^MING.* ]]; then
  WORKING_DIR=$(pwd -W)
  DOCKER_ROOT="//"
fi

HAWKEYE_RESULTS_FILE_PATH="${DOCKER_ROOT}hawkeye-results.json"

if [ 1 -eq "$(docker ps -a | grep -c $HAWKEYE_CONTAINER_NAME)" ]
then
  docker rm "$HAWKEYE_CONTAINER_NAME"
fi

docker pull "$HAWKEYE_IMAGE"

set +e

docker run \
  --name "$HAWKEYE_CONTAINER_NAME" \
  -v "$WORKING_DIR:${DOCKER_ROOT}target:ro" \
  "$HAWKEYE_IMAGE" \
  scan -f critical -j "$HAWKEYE_RESULTS_FILE_PATH"

test_run_result=$?

docker cp \
  "$HAWKEYE_CONTAINER_NAME":"$HAWKEYE_RESULTS_FILE_PATH" \
  hawkeye-results.json

if [ -n "$TF_BUILD" ] && [ -f hawkeye-results.json ]; then
  echo "##vso[artifact.upload containerfolder=web-security-scan;artifactname=hawkeyeresults]$(realpath hawkeye-results.json)"
fi

docker rm "$HAWKEYE_CONTAINER_NAME"

if [ $test_run_result -ne 0 ]; then
  die "Hawkeye security scan failed"
fi
