#!/bin/bash

set -e

IMAGES=(nhsonline-web nhsonline-backendpfsapi nhsonline-backendcidapi nhsonline-backendservicejourneyrulesapi nhsonline-backendusersapi nhsonline-backenduserinfoapi nhsonline-backendmessagesapi)

DOCKER_REGISTRY=${DOCKER_REGISTRY:-local}
DOCKER_TAG=${DOCKER_TAG:-latest}

TWISTLOCK_CLI_USER=${TWISTLOCK_CLI_USER:-twistcli}
TWISTLOCK_CLI_CONSOLE=${TWISTLOCK_CLI_CONSOLE:-https://twistlock.production.nhsapp.service.nhs.uk:8083}
TWISTLOCK_CLI_IMAGE=${TWISTLOCK_CLI_IMAGE:-nhsapp.azurecr.io/twistcli:19.11.480}

if [ -z "$TWISTLOCK_CLI_PASS" ] && [ -f "$HOME/.nhsonline/secrets/twistlock.password" ]; then
  TWISTLOCK_CLI_PASS=$(<~/.nhsonline/secrets/twistlock.password)
fi

if [ -z "$TWISTLOCK_CLI_PASS" ]; then
  echo "Twistlock password is required (TWISTLOCK_CLI_PASS or $HOME/.nhsonline/secrets/twistlock.password)"
  exit 1
fi

if [[ $(uname -s) =~ ^MING.* ]]
then
  MOUNT_DOCKER_SOCKET="//var/run/docker.sock:/var/run/docker.sock"
else
  MOUNT_DOCKER_SOCKET="/var/run/docker.sock:/var/run/docker.sock"
fi

if [ "$DOCKER_REGISTRY" != "local" ]; then
  for IMAGE in "${IMAGES[@]}"
  do
      docker pull "$DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG"
  done
fi

for IMAGE in "${IMAGES[@]}"
do
  echo
  echo "Scanning $DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG"
  echo

  docker run --rm \
    -v "$MOUNT_DOCKER_SOCKET" \
    "$TWISTLOCK_CLI_IMAGE" \
    images scan \
    -u "$TWISTLOCK_CLI_USER" -p "$TWISTLOCK_CLI_PASS" --address "$TWISTLOCK_CLI_CONSOLE" \
    --vulnerability-threshold critical \
    --details \
    "$DOCKER_REGISTRY/$IMAGE:$DOCKER_TAG"
done
