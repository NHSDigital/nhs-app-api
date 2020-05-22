#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=./lib/functions.sh
source "buildscripts/lib/functions.sh"

HAWKEYE_IMAGE="hawkeyesec/scanner-cli:latest"
HAWKEYE_CONTAINER_NAME="nhsonline-web-hawkeye"
HAWKEYE_RESULTS_FILE="hawkeye-results.json"
HAWKEYE_RESULTS_FILE_PATH="${DOCKER_ROOT}${HAWKEYE_RESULTS_FILE}"

if [ -n "$(docker container ls -a -q --filter="name=${HAWKEYE_CONTAINER_NAME}")" ]; then
  docker rm "${HAWKEYE_CONTAINER_NAME}"
fi

docker pull "${HAWKEYE_IMAGE}"

set +e

docker run \
  --name "${HAWKEYE_CONTAINER_NAME}" \
  -v "${REPO_ROOT}/web:${DOCKER_ROOT}target:ro" \
  "${HAWKEYE_IMAGE}" \
  scan -f critical \
        -j "${HAWKEYE_RESULTS_FILE_PATH}"

test_run_result=$?

docker cp \
  "${HAWKEYE_CONTAINER_NAME}:${HAWKEYE_RESULTS_FILE_PATH}" \
  "${HAWKEYE_RESULTS_FILE}"

if [ -n "${TF_BUILD}" ] && [ -r "${HAWKEYE_RESULTS_FILE}" ]; then
  echo "##vso[artifact.upload containerfolder=web-security-scan;artifactname=hawkeyeresults]$(realpath "${HAWKEYE_RESULTS_FILE}")"
fi

docker rm "${HAWKEYE_CONTAINER_NAME}"

if [ ${test_run_result} -ne 0 ]; then
  die "Hawkeye security scan failed"
fi
