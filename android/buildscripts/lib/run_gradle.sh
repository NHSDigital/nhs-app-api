#!/bin/bash

docker run \
  "${DOCKER_ARGS[@]}" \
  nhsapp.azurecr.io/android:latest \
  bash -c "cd /repo; if [ -f 'local.properties' ]; then rm -f 'local.properties'; fi; ./gradlew ${GRADLE_ARGS[*]}"
