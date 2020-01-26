#!/bin/bash

docker run \
  "${DOCKER_ARGS[@]}" \
  nhsapp.azurecr.io/android:latest \
  bash -c "cd /repo; ./gradlew ${GRADLE_ARGS[*]}"
