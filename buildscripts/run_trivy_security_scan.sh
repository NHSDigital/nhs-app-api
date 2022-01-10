#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source buildscripts/lib/set_env.sh

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

DOCKER_REGISTRY=${DOCKER_REGISTRY:-local}
DOCKER_TAG=${DOCKER_TAG:-latest}

if [[ $(uname -s) =~ ^MING.* ]]
then
  MOUNT_DOCKER_SOCKET="//var/run/docker.sock:/var/run/docker.sock"
else
  MOUNT_DOCKER_SOCKET="/var/run/docker.sock:/var/run/docker.sock"
fi

if [ "$DOCKER_REGISTRY" != "local" ]; then
  for IMAGE in "${IMAGE_NAMES[@]}"
  do
      docker pull "$DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG"
  done
fi

echo "IP Address: $(curl https://ifconfig.co/)"

for IMAGE in "${IMAGE_NAMES[@]}"
do
  echo
  echo "Scanning $DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG"
  echo

  trivy image "$DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG" || die "Trivy security scan failed for $DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG"
done
