#! /usr/bin/env bash

function set_int_test_config_from_environment () {
  local CONFIG_VAR
  local DOCKER_ENV_VAR_NAME
  local DOCKER_ENV_VAR_VALUE

  for CONFIG_VAR in "${!INT_TEST_CFG_@}"; do
    DOCKER_ENV_VAR_NAME=${CONFIG_VAR#INT_TEST_CFG_}
    DOCKER_ENV_VAR_VALUE=${!CONFIG_VAR}

    info "Setting Integration Tests Config: ${DOCKER_ENV_VAR_NAME}=${DOCKER_ENV_VAR_VALUE}"
    DOCKER_ARGS+=(--env "${DOCKER_ENV_VAR_NAME}=${DOCKER_ENV_VAR_VALUE}")
  done
}

function set_docker_compose_files_args () {
  local DOCKER_COMPOSE_FILES PORT_FILE

  DOCKER_COMPOSE_FILES=(../docker-compose.yml)
  DOCKER_COMPOSE_FILES+=(../docker/stubbed/docker-compose.yml ../docker/docker-compose.nhslogin-stubbed.yml)
  DOCKER_COMPOSE_FILES+=(../docker/bddtests/docker-compose.yml ../docker/inttests/docker-compose.yml)
  DOCKER_COMPOSE_FILES+=("${DOCKER_COMPOSE_FILES_TRANCHE[@]}")

  if [ "$RUN_LOCAL" == 1 ]
  then
    DOCKER_COMPOSE_FILES+=(../docker/android/docker-compose.yml ../docker-compose.ports.yml  ../docker/inttests/docker-compose.local.yml)
    for PORT_FILE in $(env | grep _DOCKER_PORTS | sed 's#^.*_DOCKER_PORTS=#../docker/#'); do
      DOCKER_COMPOSE_FILES+=("$PORT_FILE")
    done
  fi

  info "Docker compose files: " "${DOCKER_COMPOSE_FILES[@]}"

  DOCKER_COMPOSE_FILES_ARGS=()
  for file in ${DOCKER_COMPOSE_FILES[*]}; do
    DOCKER_COMPOSE_FILES_ARGS+=(-f "$file")
  done
}

function start_services_under_test () {
  if [ -z "$TF_BUILD" ]; then
    # shellcheck source=../../../buildscripts/lib/generate_host_settings.sh
    source ../buildscripts/lib/generate_host_settings.sh

    generate_host_settings
  fi

  if [ "$RUN_LOCAL" == 1 ]; then
    docker-compose -p "$DOCKER_PROJECT_NAME" "${DOCKER_COMPOSE_FILES_ARGS[@]}" up
    exit
  fi

  docker-compose -p "$DOCKER_PROJECT_NAME" "${DOCKER_COMPOSE_FILES_ARGS[@]}" up -d || die "Docker compose failure"
}

function stop_services_under_test () {
  if [ -z "$TF_BUILD" ]; then
    docker stop "${DOCKER_PROJECT_NAME}_test_runner"
    docker-compose -p "$DOCKER_PROJECT_NAME" "${DOCKER_COMPOSE_FILES_ARGS[@]}" stop
  fi
}

function destroy_services_under_test () {
  if [ -z "$TF_BUILD" ]; then
    docker rm "${DOCKER_PROJECT_NAME}_test_runner"
    docker-compose -p "$DOCKER_PROJECT_NAME" "${DOCKER_COMPOSE_FILES_ARGS[@]}" down --volume
  fi
}

function fetch_container_logs () {
  local CONTAINER_ID CONTAINER_NAME

  mkdir -p logs

  for CONTAINER_ID in $(docker ps -aq --filter=network="$DOCKER_NETWORK"); do
    CONTAINER_NAME=$(docker ps -a --filter "id=$CONTAINER_ID" --format '{{.Names}}')
    info "Fetching logs for $CONTAINER_NAME ($CONTAINER_ID)"
    docker logs "$CONTAINER_NAME" >& "./logs/$CONTAINER_NAME.log"
  done
}
