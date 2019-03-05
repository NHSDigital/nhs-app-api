#!/bin/bash

displayUsage () {
  echo "Usage: ./build_and_test.sh [-A] [-D] [-C] [TAGLIST]

  Build and run the BDD test stack within docker to mimic the TeamCity process
  TAGLIST is a collection of tags from the BDD tests and should include the @ prefix
  Example: ./build_and_test.sh -A @NHSO-1704 @prescription

  Flags:
    -A     Run the specified tags against all available tests
               If this is specified then you MUST also supply at least one tag
               [Default is to run tags against a subset depending on branch]
    -C     Execute T&C BDD Tests Only - Uses Shared Cosmos Instance
    -D     Force a branch build to run as if it was Develop and execute full BDD suite
               [This option is ignored if -A is specified]
    -L     Force a branch build to run as if it was Develop and execute full BDD suite including long running tests
                [This option is ignored if -A is specified]
    -P     Run tests in parallel (WARNING: This can consume a large amount of docker VM memory)
    -h     Display this help message
  "
  exit 1
}

RUN_SUBSET=1
PARALLEL=0
RUN_AS_DEVELOP=0
ENABLE_LONG_RUNNING=0
PREFIX="and"
COSMOS_AUTHKEY=""
ENABLE_COSMOS_TESTS=0

while getopts "ADCPh" opt; do

  case $opt in
    A)
      PREFIX="--tags"
      RUN_SUBSET=0
      ;;
    D)
      RUN_AS_DEVELOP=1
      ;;
     C)
      ENABLE_COSMOS_TESTS=1
      ;;
    L)
      RUN_AS_DEVELOP=1
      ENABLE_LONG_RUNNING=1
      ;;
    P)
      PARALLEL=1
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

CHROME_IMAGE=chrome:latest
DOCKER_REGISTRY=nhsapp.azurecr.io
WEB_NAME=nhsonline-web
CID_BACKEND_NAME=nhsonline-backendcidapi
PFS_BACKEND_NAME=nhsonline-backendpfsapi
TAG=$(git rev-parse HEAD)

cd bddtests/ops

if [ "$ENABLE_COSMOS_TESTS" == 1 ]
then
  if [ ! -f testcosmosauthkey.txt ]; then
    echo "You must copy testcosmosauthkey.txt file from keybase to ops folder before running tests tagged with @cosmos"
    exit 1
  fi
  COSMOS_AUTHKEY=$(<testcosmosauthkey.txt)
fi

docker pull $DOCKER_REGISTRY/$CHROME_IMAGE

cd ../../web
docker build . -t $DOCKER_REGISTRY/$WEB_NAME:$TAG -f Dockerfile
docker tag $DOCKER_REGISTRY/$WEB_NAME:$TAG $DOCKER_REGISTRY/$WEB_NAME:latest

cd ../backendworker
docker build . -t $DOCKER_REGISTRY/$CID_BACKEND_NAME:$TAG -f NHSOnline.Backend.CidApi/Dockerfile
docker tag $DOCKER_REGISTRY/$CID_BACKEND_NAME:$TAG $DOCKER_REGISTRY/$CID_BACKEND_NAME:latest

docker build . -t $DOCKER_REGISTRY/$PFS_BACKEND_NAME:$TAG -f NHSOnline.Backend.PfsApi/Dockerfile
docker tag $DOCKER_REGISTRY/$PFS_BACKEND_NAME:$TAG $DOCKER_REGISTRY/$PFS_BACKEND_NAME:latest

cd ../bddtests/ops
PARALLEL=$PARALLEL RUN_AS_DEVELOP=$RUN_AS_DEVELOP RUN_SUBSET=$RUN_SUBSET ENABLE_LONG_RUNNING=$ENABLE_LONG_RUNNING SPECIFIC_TEST_TAGS=$SPECIFIC_TEST_TAGS APP_DOCKER_TAG=$TAG DOCKER_REGISTRY=$DOCKER_REGISTRY ENABLE_COSMOS_TESTS=$ENABLE_COSMOS_TESTS COSMOS_AUTHKEY=$COSMOS_AUTHKEY ./docker_tests.sh
