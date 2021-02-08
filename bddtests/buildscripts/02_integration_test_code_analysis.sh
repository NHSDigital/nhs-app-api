#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

GRADLE_TASKS=()

configure_env() {
  if [ -z "$TF_BUILD" ]; then
    GRADLE_TASKS+=(clean)
  fi

  GRADLE_TASKS+=(detektCheck)
  GRADLE_TASKS+=(lintGherkin)
  GRADLE_TASKS+=(findUnusedSteps)
}

run_analysis() {
  validate_maven_settings

  validate_npm_settings

  configure_env

  pull_docker_image "${DOCKER_IMAGE_GRADLE}"

  rebuild_image_with_user "${DOCKER_IMAGE_GRADLE}"

  configure_npmrc_and_m2_volumes

  docker run --rm \
    "${DOCKER_ARGS[@]}" \
    "${DOCKER_IMAGE_GRADLE}" \
    bash -c "\
      set -e; \
      ./gradlew --no-daemon ${GRADLE_TASKS[*]} \
    " || die "Integration Tests Code Analysis Failed"
}

check_for_unused_steps() {
  local UNUSED_STEPS

  if [ -f "target/Unused-Steps.txt" ]; then
    IFS=$'\r\n' GLOBIGNORE='*' command eval "UNUSED_STEPS=(\$(<target/Unused-Steps.txt))"

    if [ ${#UNUSED_STEPS[@]} -gt 1 ]; then
      for UNUSED_STEP in "${UNUSED_STEPS[@]:1:5}"; do
        error "Unused Step: $UNUSED_STEP"
      done

      die "${UNUSED_STEPS[0]/:/}"
    fi
  fi
}

if [ "$SKIP_ANALYSIS" != 1 ] && [ "$RUN_LOCAL_BDD" != 1 ]; then
  run_analysis
  check_for_unused_steps
fi
