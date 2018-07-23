#!/bin/bash

displayUsage () {
  echo "Usage: ./build_and_test.sh [-A] [TAGLIST]
  
  Build and run the BDD test stack within docker to mimic the TeamCity process
  TAGLIST is a collection of tags from the BDD tests and should include the @ prefix
  Example: ./build_and_test.sh -A @NHSO-1704 @prescription
  
  Flags:
    -A     Run the specified tags against all available tests
               If this is specified then you MUST also supply at least one tag
               [Default is to run tags against a subset depending on branch]
    -h     Display this help message
  "
  exit 1
}

RUN_SUBSET=1
PREFIX="and"

while getopts "Ah" opt; do

  case $opt in 
    A)
      PREFIX="--tags"
      RUN_SUBSET=0
      ;;
    h)
      displayUsage
      ;;
    \?)
      displayUsage
      ;;
  esac
done

shift $((OPTIND-1))

for var in "$@"
do
    if ! [[ $var =~ ^@.* ]]
    then
      echo "Error: Tags must be prefixed with @" >&2
      displayUsage
    fi
done

if [ $RUN_SUBSET -eq 0 ] && [ $# == 0 ]
then
  echo "Error: Run against all tests specified with no tag selector" >&2
  displayUsage
fi

if [ $# != 0 ]
then
  SPECIFIC_TEST_TAGS="$PREFIX $*"
fi

ROOT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $ROOT_DIR

DOCKER_REGISTRY=nhsapp.azurecr.io
WEB_NAME=nhsonline-web
BACKEND_NAME=nhsonline-backendworker
TAG=$(git rev-parse HEAD)

cd web
docker build . -t $DOCKER_REGISTRY/$WEB_NAME:$TAG -f Dockerfile
docker tag $DOCKER_REGISTRY/$WEB_NAME:$TAG $DOCKER_REGISTRY/$WEB_NAME:latest

cd ../backendworker
docker build . -t $DOCKER_REGISTRY/$BACKEND_NAME:$TAG -f NHSOnline.Backend.Worker/Dockerfile
docker tag $DOCKER_REGISTRY/$BACKEND_NAME:$TAG $DOCKER_REGISTRY/$BACKEND_NAME:latest

cd ../bddtests/ops
RUN_SUBSET=$RUN_SUBSET SPECIFIC_TEST_TAGS=$SPECIFIC_TEST_TAGS APP_DOCKER_TAG=$TAG DOCKER_REGISTRY=$DOCKER_REGISTRY ./docker_tests.sh
