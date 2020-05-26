#! /usr/bin/env bash

function cleanup_docker_containers () {
  local CONTAINER

  if [ -z "$TF_BUILD" ]; then # Azure DevOps creates clean build agents so no need to clean
    info "Output currently executing docker containers to aid debug"
    docker ps

    info "Cleaning up hanging containers"
    for CONTAINER in $(docker ps -q --filter=name="${DOCKER_PROJECT_NAME}_*"); do
      docker stop "$CONTAINER"
    done
    for CONTAINER in $(docker ps -qa --filter=name="${DOCKER_PROJECT_NAME}_*"); do
      docker rm "$CONTAINER"
    done
  fi
}

function set_docker_compose_files_args () {
  local DOCKER_COMPOSE_FILES PORT_FILE

  DOCKER_COMPOSE_FILES=(../docker-compose.yml ../docker/stubbed/docker-compose.yml ../docker/bddtests/docker-compose.yml "${DOCKER_COMPOSE_FILES_TRANCHE[@]}")

  if [ "$RUN_LOCAL_BDD" == 1 ]
  then
    DOCKER_COMPOSE_FILES+=(../docker-compose.ports.yml)
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
  fi
}

function start_services_under_test () {
  if [ -z "$TF_BUILD" ]; then
    # shellcheck source=../../../buildscripts/lib/set_env.sh
    source ../buildscripts/lib/set_env.sh
    # shellcheck source=../../../buildscripts/lib/generate_host_settings.sh
    source ../buildscripts/lib/generate_host_settings.sh

    generate_host_settings
  fi

  if [ "$RUN_LOCAL_BDD" == 1 ]; then
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

function rebuild_image_with_user() {
  baseImage="$1"

  if [ -z "$TF_BUILD" ] && [ -z "$NO_PULL" ]; then
    docker pull "${baseImage}"
  fi

  if ! [[ $(uname -s) =~ ^Linux.* ]]; then
    # permission only needs to be fixed on linux hosts
    return
  fi

  docker build \
    -t "${baseImage}" \
    --build-arg "BASE_IMAGE=${baseImage}" \
    --build-arg "USER_NAME=${USER}" \
    --build-arg "USER_ID=$(id -u)" \
    --build-arg "GROUP_ID=$(id -g)" \
    "buildscripts/changeuser" || die "Failed to build docker image with Linux permissions"
}

function configure_npmrc_and_m2_volumes () {
  if [ -z "${TF_BUILD}" ] && [[ "${BROWSER}" =~ ^browserstack_* ]]; then
    DOCKER_USER="browserstack"
  fi

  DOCKER_ARGS+=(-v "${MVN_CFG_PATH}:${DOCKER_ROOT}home/${DOCKER_USER}/.m2/settings.xml")
  DOCKER_ARGS+=(-v "${NPMRC_PATH}:${DOCKER_ROOT}home/${DOCKER_USER}/.npmrc")
}
