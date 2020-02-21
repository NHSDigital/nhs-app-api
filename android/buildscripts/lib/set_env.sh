#! /usr/bin/env bash

CURRENT_DIR=$(pwd)
MVN_CONFIG_PATH="${HOME}/.m2/settings.xml"
GRADLE_PATH="${HOME}/.gradle"

DOCKER_ARGS=(--rm)
DOCKER_ROOT="/"

if [[ $(uname -s) =~ ^MING.* ]]; then
  CURRENT_DIR=$(pwd -W)
  MVN_CONFIG_PATH="${USERPROFILE}/.m2/settings.xml"
  GRADLE_PATH="${USERPROFILE}/.gradle"
  DOCKER_ROOT="//"
fi

mkdir -p "${GRADLE_PATH}"

DOCKER_ARGS+=(-v "${CURRENT_DIR}:${DOCKER_ROOT}data/repo")
DOCKER_ARGS+=(-v "${MVN_CONFIG_PATH}:${DOCKER_ROOT}root/.m2/settings.xml")
DOCKER_ARGS+=(-v "${GRADLE_PATH}:${DOCKER_ROOT}data/.gradle")

DOCKER_ARGS+=(-w "${DOCKER_ROOT}data/repo")

DOCKER_ARGS+=(-e "GRADLE_USER_HOME=${DOCKER_ROOT}data/.gradle")

GRADLE_ARGS=("-Dorg.gradle.jvmargs='-XX:+UnlockExperimentalVMOptions -XX:+UseCGroupMemoryLimitForHeap'")

export DOCKER_ARGS
export GRADLE_ARGS
