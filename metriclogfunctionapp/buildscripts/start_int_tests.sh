#! /usr/bin/env bash

set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

function print_docker_logs () {
  local NAME
  for NAME in $(docker ps -aq --format "{{.Names}}"); do
    info "Container Logs $NAME"
    docker logs "$NAME"
  done
}

docker build -t local/metriclog-function . || die "Failed to build metriclog function docker image"

if [ "$RUN_LOCAL" == "1" ]; then
  ./buildscripts/run_docker_compose.sh docker-compose.yml docker/inttests/docker-compose.yml
else
  PWD=$(pwd)
  if [[ $(uname -s) =~ ^MING.* ]]; then
    PWD=$(pwd -W)
  fi

  DOCKER_COMPOSE_ARGS=(-f docker-compose.yml -f docker/inttests/docker-compose.yml)

  if on_build_agent; then
    DOCKER_COMPOSE_ARGS+=(-f docker/inttests/docker-compose.host-docker-internal.yml)
  fi

  docker-compose "${DOCKER_COMPOSE_ARGS[@]}" up -d || die "docker-compose up failed"

  MAX_WAIT_TIME_SECS=120
  START_TIME=$(date +%s)

  STARTED=0
  until [ $STARTED -eq 1 ]; do
    if docker logs metriclog_postgres_1 2> /dev/null ; then
      echo "Started"
      STARTED=1
    else
      NOW=$(date +%s)
      WAITED=$((NOW-START_TIME))
      if [ $WAITED -gt $MAX_WAIT_TIME_SECS ]; then
        echo "Waited ${WAITED}s for start, giving up"
        exit 1
      fi
      echo "Waiting for Postgres to start (${WAITED}s elapsed)"
      sleep 2
    fi
  done

  START_TIME=$(date +%s)

  STARTED=0
  until [ $STARTED -eq 1 ]; do
    if docker logs metriclog_function_1 | grep -q 'Host lock lease acquired by instance ID'; then
      echo "Started"
      STARTED=1
    else
      NOW=$(date +%s)
      WAITED=$((NOW-START_TIME))
      if [ $WAITED -gt $MAX_WAIT_TIME_SECS ]; then
        echo "Waited ${WAITED}s for start, giving up"
        exit 1
      fi
      echo "Waiting for Metric Logger to start (${WAITED}s elapsed)"
      sleep 2
    fi
  done

  EXPECTED_CONTAINERS=(metriclog_storage_1 metriclog_postgres_1 metriclog_function_1)
  MISSING_CONTAINERS=()
  for CONTAINER_NAME in "${EXPECTED_CONTAINERS[@]}"; do
    if [[ -z "$(docker ps -qf name="$CONTAINER_NAME")" ]]; then
      MISSING_CONTAINERS+=("$CONTAINER_NAME")
      error "Container $CONTAINER_NAME not started";
      info "Container Logs $CONTAINER_NAME"
      docker logs "$CONTAINER_NAME"
    fi;
  done

  if [ ${#MISSING_CONTAINERS[@]} -gt 0 ]; then
    die "Not all containers are running, missing ${MISSING_CONTAINERS[*]}"
  fi
fi
