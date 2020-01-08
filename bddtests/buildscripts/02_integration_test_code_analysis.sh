#!/bin/bash

set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

if [ "$SKIP_ANALYSIS" == 1 ]; then
  exit
fi

GRADLE_TASKS=()
if [ -z "$TF_BUILD" ]; then
  GRADLE_TASKS+=(clean)
fi
GRADLE_TASKS+=(detektCheck)
GRADLE_TASKS+=(lintGherkin)

docker pull "$DOCKER_IMAGE_GRADLE"

docker run \
  --rm \
  -v "$WORKING_DIR:/repo" \
  "$DOCKER_IMAGE_GRADLE" bash -c " \
    set -e ; \
    cd /repo ; \
    ./gradlew ${GRADLE_TASKS[*]}; \
    chown -R $USER_ID:$GROUP_ID /repo"
