#!/bin/bash

DOCKER_ARGS=(--rm)
if [[ $(uname -s) =~ ^MING.* ]]
then
  DOCKER_ARGS+=(-v "$(pwd -W)/..://repo")
  DOCKER_ARGS+=(-v "$USERPROFILE/.m2/settings.xml://root/.m2/settings.xml")
else
  DOCKER_ARGS+=(-v "$(pwd)/..:/repo")
  DOCKER_ARGS+=(-v "$HOME/.m2/settings.xml:/root/.m2/settings.xml")
fi

GRADLE_ARGS=("-Dorg.gradle.jvmargs='-XX:+UnlockExperimentalVMOptions -XX:+UseCGroupMemoryLimitForHeap'")

export DOCKER_ARGS
export GRADLE_ARGS
