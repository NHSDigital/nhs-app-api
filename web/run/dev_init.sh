#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../run/lib/set_env.sh
source "run/lib/set_env.sh"

docker build --target dev-serve -t local/nhsonline-web-serve:latest .

[ -z "$(docker volume list -f "name=web-node-modules" -q)" ] || docker volume rm web-node-modules
docker volume create web-node-modules

docker run -it --rm \
  --name web-init \
  "${DOCKER_ARGS[@]}" \
  local/nhsonline-web-serve:latest npm install
