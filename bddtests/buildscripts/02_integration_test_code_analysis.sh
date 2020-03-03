#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
. "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
. "buildscripts/lib/functions.sh"

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

  docker run --rm \
    "${DOCKER_ARGS[@]}" \
    "${DOCKER_IMAGE_GRADLE}" \
    bash -c "./gradlew --no-daemon ${GRADLE_TASKS[*]}" || die "Integration Tests Code Analysis Failed"
}

if [ "$SKIP_ANALYSIS" != 1 ]; then
  runAnalysis
fi
