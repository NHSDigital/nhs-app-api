#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../run/lib/set_env.sh
source "run/lib/set_env.sh"

docker run -it --rm \
  --name web-serve \
  "${DOCKER_ARGS[@]}" \
  local/nhsonline-web-serve:latest bash
