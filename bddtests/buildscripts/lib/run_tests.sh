#! /usr/bin/env bash

validate_maven_settings

validate_azure_notification_hub_key

validate_npm_settings

cleanup_docker_containers

set_docker_compose_files_args

pull_docker_images

start_services_under_test

pull_docker_image "${DOCKER_IMAGE}"

rebuild_image_with_user "${DOCKER_IMAGE}"

configure_npmrc_and_m2_volumes

docker run \
  --name "${DOCKER_PROJECT_NAME}_test_runner" \
  --network "${DOCKER_NETWORK}" \
  --env-file ../docker/bddtests/env/vars_test_runner.env \
  "${DOCKER_ARGS[@]}" \
  "${TRANCHE_RUN_ADDITIONAL_ARGS[@]}" \
  "${DOCKER_IMAGE}" bash -c " \
    set -e; \
    ${TRANCHE_RUN_SETUP} \
    ./gradlew --no-daemon ${TRANCHE_RUN_GRADLE_TASKS[*]} \
      -Dwebdriver.provided.type=${BROWSER} \
      -Dcucumber.options=\"--strict --tags '${TAGS}' \" \
      ${TRANCHE_RUN_GRADLE_ARGS[*]}"

stop_services_under_test

fetch_container_logs

destroy_services_under_test

if [ -f "build/failures.txt" ]; then
  FAILED_TESTS=$(wc -l build/failures.txt | awk '{ print $1 }')
  if [ "$FAILED_TESTS" -lt 5 ]; then
    grep -v '^ *#' < build/failures.txt | while IFS= read -r TEST_NAME; do
      error "Failed: $TEST_NAME"
    done
  fi

  die "${TESTS_NAME:-$TRANCHE_TAG} $FAILED_TESTS Integration Tests Failed"
fi
