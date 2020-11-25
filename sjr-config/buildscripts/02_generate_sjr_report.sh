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

mkdir -p $WORKING_DIR/configurations/export
chmod 777 $WORKING_DIR/configurations/export

docker run -e "SJR_ARGS=--export-only" \
    -e "OUTPUT_FOLDER_PATH=$SJR_CONFIGURATIONS/output" \
    -e "CSV_EXPORT_OUTPUT_FILE_PATH=$SJR_CONFIGURATIONS/export/ods-code-summary.csv" \
    -v $WORKING_DIR/configurations/:$SJR_CONFIGURATIONS \
    ${DOCKER_REGISTRY}/$IMAGE:$SJR_IMAGE_TAG \
    dotnet NHSOnline.Backend.ServiceJourneyRulesApi.dll --export-only
