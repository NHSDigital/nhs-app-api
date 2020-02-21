#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
. "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions_docker.sh
. "buildscripts/lib/functions_docker.sh"

GRADLE_TASKS=()

configureEnv() {
  if [ -z "$TF_BUILD" ]; then
    GRADLE_TASKS+=(clean)
  fi

  GRADLE_TASKS+=(detektCheck)
  GRADLE_TASKS+=(lintGherkin)
}

runAnalysis() {
  configureEnv
  rebuild_image_with_user "${DOCKER_IMAGE_GRADLE}"

  docker run \
    --rm \
    -v "${WORKING_DIR}:${DOCKER_ROOT}data/repo" \
    -v "${GRADLE_PATH}:${DOCKER_ROOT}data/.gradle" \
    -w "/data/repo" \
    -e "GRADLE_USER_HOME=/data/.gradle" \
    "${DOCKER_IMAGE_GRADLE}" \
    bash -c "./gradlew --no-daemon ${GRADLE_TASKS[*]}"
}

if [ "$SKIP_ANALYSIS" != 1 ]; then
  runAnalysis
fi
