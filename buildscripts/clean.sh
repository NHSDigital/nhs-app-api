#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.."

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

clean() {
  info "Clean started"

  CONTAINERS=()
  while IFS='' read -r line; do CONTAINERS+=("$line"); done < <(docker ps -a -q)

  if [ ${#CONTAINERS[@]} -ne 0 ]; then
    info "Docker containers found, removing..."

    docker rm -f "${CONTAINERS[@]}"
  fi

  LOCAL_IMAGES=()
  while IFS='' read -r line; do LOCAL_IMAGES+=("$line"); done < <(docker image list --filter=reference=local/nhsonline* -q)

  if [ ${#LOCAL_IMAGES[@]} -ne 0 ]; then
    info "Docker local images found, removing..."

    docker rmi -f "${LOCAL_IMAGES[@]}"
  fi

  docker system prune --volumes --force

  info "Clean finished"
}

clean
