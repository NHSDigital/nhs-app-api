#!/bin/bash
set -e -u

VOLUME_NAME="nhsapp-devops-tools-home-dir"
IMAGE_TAG="${TAG:-31373}"


if ! docker volume inspect "$VOLUME_NAME" &>/dev/null; then
    docker volume create "$VOLUME_NAME"
fi

# MSYS_NO_PATHCONV=1 prevents git bash for Windows from mangling the /root path
MSYS_NO_PATHCONV=1 \
    docker run -it --rm \
        --mount "source=${VOLUME_NAME},target=/root" \
        "nhsapp.azurecr.io/nhsapp-devops-tools:$IMAGE_TAG"
