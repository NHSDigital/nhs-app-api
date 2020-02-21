#! /usr/bin/env bash

validate_azure_notification_hub_key

cleanup_docker_containers

set_docker_compose_files_args

pull_docker_images

start_services_under_test

rebuild_image_with_user "${DOCKER_IMAGE}"

docker run \
  --name "${DOCKER_PROJECT_NAME}_test_runner" \
  --network "${DOCKER_NETWORK}" \
  --env-file ../docker/bddtests/env/vars_test_runner.env \
  -v "${WORKING_DIR}:${DOCKER_ROOT}data/repo" \
  -v "${GRADLE_PATH}:${DOCKER_ROOT}data/.gradle" \
  -w "/data/repo" \
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
  info "Tests failed"
  cat build/failures.txt
  exit 1
fi
