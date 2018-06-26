#!/usr/bin/env bash

DOCKER_IMAGE=microsoft/dotnet:2.1-sdk

HOST_PATH=$(pwd)
CONTAINER_PATH=/repo

docker run \
  --rm \
  -v $HOST_PATH:$CONTAINER_PATH $DOCKER_IMAGE \
  /bin/bash -c "  \
  cd /repo ; \
  dotnet test NHSOnline.Backend.Worker.UnitTests"
