#!/bin/bash

validate_azure_notification_hub_key

cleanup_docker_containers

set_docker_compose_files_args

pull_docker_images

start_services_under_test

docker run \
  --name "${DOCKER_PROJECT_NAME}_test_runner" \
  --network "$DOCKER_NETWORK" \
  --env-file ../docker/bddtests/env/vars_test_runner.env \
  -v "$WORKING_DIR:/repo" \
  "${TRANCHE_RUN_ADDITIONAL_ARGS[@]}" \
  "$DOCKER_IMAGE" bash -c " \
    set -e ; \
    cd /repo ; \
    $TRANCHE_RUN_SETUP \
    ./gradlew ${TRANCHE_RUN_GRADLE_TASKS[*]} \
      -Dwebdriver.provided.type=$BROWSER \
      -Dcucumber.options=\"--strict --tags '$TAGS' \" \
      ${TRANCHE_RUN_GRADLE_ARGS[*]}; \
    chown -R $USER_ID:$GROUP_ID /repo"

stop_services_under_test

fetch_container_logs

destroy_services_under_test

if [ -f "build/failures.txt" ]; then
  info "Tests failed"
  cat build/failures.txt
  exit 1
fi
