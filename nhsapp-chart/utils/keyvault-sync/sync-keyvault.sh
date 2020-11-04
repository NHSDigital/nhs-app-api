#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/../../.."

. "buildscripts/lib/set_env.sh"

utilsPath="${REPO_ROOT}/nhsapp-chart/utils/keyvault-sync"
profilePath="${HOME_PATH}/.azure/pwsh"

if [ -z "$(docker image ls | grep 'nhsapp-az-pwsh')" ]; then
  docker build -t nhsapp-az-pwsh "${utilsPath}"
fi

mkdir -p "${profilePath}"

docker run --rm -it \
  -v "${profilePath}:${DOCKER_ROOT}root/.Azure" \
  -v "${utilsPath}:${DOCKER_ROOT}temp/script" \
  nhsapp-az-pwsh "/temp/script/sync-keyvault.ps1"
