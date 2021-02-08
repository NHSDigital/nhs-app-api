#! /usr/bin/bash

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

function pull_docker_images () {
  local DOCKER_IMAGES IMAGE_TO_PULL

  DOCKER_IMAGES=$(docker-compose "${DOCKER_COMPOSE_FILES_ARGS[@]}" config | grep image | sed -e 's/^[[:space:]]*image: //' | sort -u)
  info "Configured images:
  $DOCKER_IMAGES"

  for IMAGE_TO_PULL in $DOCKER_IMAGES; do
    pull_docker_image "$IMAGE_TO_PULL"
  done
}

function pull_docker_image () {
  local IMAGE_TO_PULL=$1
  local MAX_RETRIES

  if [ -n "$NO_PULL" ]; then
    return
  fi

  if [ -z "$TF_BUILD" ]; then
    MAX_RETRIES=0
  else
    MAX_RETRIES=3
  fi

  case "$IMAGE_TO_PULL" in
    local/*) echo "Skipping pull on local image $IMAGE_TO_PULL";;
    *)
      info "Pulling docker image $IMAGE_TO_PULL"
      RETRIES=0
      while ! docker pull "$IMAGE_TO_PULL"; do
        if [[ $RETRIES -ge $MAX_RETRIES ]]; then
          die "Failed to pull docker image $IMAGE_TO_PULL"
        else
          warn "Failed to pull docker image $IMAGE_TO_PULL"
        fi
        RETRIES=$((RETRIES+1))
        info "Retrying $RETRIES/$MAX_RETRIES"
      done
      ;;
  esac
}
