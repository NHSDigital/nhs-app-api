#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.."

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

clean() {
  info "Clean started"

  containers=$(docker ps -a -q)
  volumes=$(docker volume ls -q)

  if [ -n "${containers}" ]; then
    info "Docker containers found, removing..."

    docker rm -f ${containers}
  fi

  if [ -n "${volumes}" ]; then
    info "Docker volumes found, removing..."

    docker volume rm -f ${volumes}
  fi

  info "Clean finished"
}

clean
