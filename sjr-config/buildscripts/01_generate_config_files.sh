#!/bin/bash

set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

IMAGE=nhsonline-backendservicejourneyrulesapi
DOCKER_REGISTRY=${DOCKER_REGISTRY:-local}
SJR_IMAGE_TAG=${SJR_IMAGE_TAG:-latest}
SJR_CONFIGURATIONS=/prodconfigurations

if [[ $(uname -s) =~ ^MING.* ]]
then
  WORKING_DIR=$(pwd -W)
else
  WORKING_DIR=$(pwd)
fi

mkdir -p $WORKING_DIR/configurations/output
chmod 777 $WORKING_DIR/configurations/output

# create Globals dir if it does not already exist
mkdir -p $WORKING_DIR/configurations/Globals

docker run -e "SJR_ARGS=--validate-only" \
    -e "GP_INFO_FILE_PATH=$SJR_CONFIGURATIONS/gpinfo.csv" \
    -e "CONFIGURATION_FOLDER_PATH=$SJR_CONFIGURATIONS" \
    -e "OUTPUT_FOLDER_PATH=$SJR_CONFIGURATIONS/output" \
    -e "JOURNEYS_FOLDER_NAME=Journeys" \
    -e "RULES_FOLDER_NAME=Rules" \
    -e "GLOBALS_FOLDER_NAME=Globals" \
    -v $WORKING_DIR/configurations/:$SJR_CONFIGURATIONS \
    ${DOCKER_REGISTRY}/$IMAGE:$SJR_IMAGE_TAG \
    dotnet NHSOnline.Backend.ServiceJourneyRulesApi.dll --validate-only
