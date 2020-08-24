#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../run/lib/set_env.sh
source "run/lib/set_env.sh"

NETWORK=${NETWORK:-nhsapp_default}
PORT=${PORT:-3000}

DOCKER_ARGS+=('--network' "$NETWORK")
DOCKER_ARGS+=('-p' "$PORT:$PORT")

if [[ "$(uname -s)" =~ ^MING.* ]]; then
  DOCKER_ARGS+=('-e' 'WEBPACK_POLL=true')
fi

CONTAINERS_ON_PORT=$(docker ps -q --filter "publish=$PORT")
[ -z "$CONTAINERS_ON_PORT" ] || docker stop "$CONTAINERS_ON_PORT"

docker run -it --rm \
  --name web-serve \
  "${DOCKER_ARGS[@]}" \
  local/nhsonline-web-serve:latest
