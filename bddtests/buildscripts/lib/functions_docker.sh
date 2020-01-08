#!/bin/bash

function cleanup_docker_containers () {
  local CONTAINER

  if [ -z "$TF_BUILD" ]; then # Azure DevOps creates clean build agents so no need to clean
    info "Output currently executing docker containers to aid debug"
    docker ps

    info "Cleaning up hanging containers"
    for CONTAINER in $(docker ps | grep -v clair | grep -v CONTAINER | awk '{print $1}'); do
      docker kill "$CONTAINER"
    done
  fi
}

function set_docker_compose_files_args () {
  local DOCKER_COMPOSE_FILES PORT_FILE

  DOCKER_COMPOSE_FILES=(../docker-compose.yml ../docker/bddtests/docker-compose.yml "${DOCKER_COMPOSE_FILES_TRANCHE[@]}")

  if [ "$RUN_LOCAL_BDD" == 1 ]
  then
    DOCKER_COMPOSE_FILES+=(../docker-compose.ports.yml ../docker/bddtests/docker-compose.ports.yml)
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

function pull_docker_images () {
  local DOCKER_IMAGES IMAGE_TO_PULL

  DOCKER_IMAGES=$(docker-compose "${DOCKER_COMPOSE_FILES_ARGS[@]}" config | grep image | sed -e 's/^[[:space:]]*image: //' | sort -u)
  info "Configured images:
  $DOCKER_IMAGES"

  if [ -z "$TF_BUILD" ] && [ -z "$NO_PULL" ]; then
    for IMAGE_TO_PULL in $DOCKER_IMAGES; do
      case "$IMAGE_TO_PULL" in
        local/*) echo "Skipping pull on local image $IMAGE_TO_PULL";;
        *) docker pull "$IMAGE_TO_PULL";;
      esac
    done

    for IMAGE_TO_PULL in "${DOCKER_TEST_RUN_IMAGES[@]}"; do
      info "Pulling image $IMAGE_TO_PULL"
      docker pull "$IMAGE_TO_PULL"
    done
  fi
}

function start_services_under_test () {
  if [ "$RUN_LOCAL_BDD" == 1 ]; then
    docker-compose -p "$TRANCHE_TAG" "${DOCKER_COMPOSE_FILES_ARGS[@]}" up
    exit
  fi

  docker-compose -p "$TRANCHE_TAG" "${DOCKER_COMPOSE_FILES_ARGS[@]}" up -d || die "Docker compose failure"

  export DOCKER_NETWORK="${TRANCHE_TAG}_default"
}

function stop_services_under_test () {
  if [ -z "$TF_BUILD" ]; then
    docker-compose -p "$TRANCHE_TAG" "${DOCKER_COMPOSE_FILES_ARGS[@]}" stop
  fi
}

function destroy_services_under_test () {
  if [ -z "$TF_BUILD" ]; then
    docker-compose -p "$TRANCHE_TAG" "${DOCKER_COMPOSE_FILES_ARGS[@]}" down
  fi
}

function fetch_container_logs () {
  local CONTAINER_ID CONTAINER_NAME

  mkdir -p logs

  for CONTAINER_ID in $(docker-compose -p "$TRANCHE_TAG" "${DOCKER_COMPOSE_FILES_ARGS[@]}" ps -q); do
    CONTAINER_NAME=$(docker ps -a --filter "id=$CONTAINER_ID" --format '{{.Names}}')
    info "Fetching logs for $CONTAINER_NAME ($CONTAINER_ID)"
    docker logs "$CONTAINER_NAME" >& "./logs/$CONTAINER_NAME.log"
  done
}