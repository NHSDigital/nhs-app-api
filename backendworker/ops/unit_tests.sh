#!/usr/bin/env bash

DOCKER_IMAGE=microsoft/dotnet:2.1-sdk

HOST_PATH=$(pwd)
CONTAINER_PATH=/repo
COVERAGE_FOLDER=NHSOnline.Backend.PfsApi.UnitTests

docker run \
  --rm \
  -v $HOST_PATH:$CONTAINER_PATH $DOCKER_IMAGE \
  /bin/bash -c "  \
    cd /repo ; \
    rm -rf $COVERAGE_FOLDER/coverage ; \
    apt-get update; \
    apt-get install -y xsltproc; \
    ./ops/unit_tests_locally.sh; \
    dotnet ~/.nuget/packages/reportgenerator/4.0.2/tools/netcoreapp2.0/ReportGenerator.dll '-reports:$COVERAGE_FOLDER/coverage/coverage.opencover.xml' '-targetdir:$COVERAGE_FOLDER/coverage' ; \
    mv $COVERAGE_FOLDER/coverage/index.htm $COVERAGE_FOLDER/coverage/index.html ; \
    xsltproc $COVERAGE_FOLDER/opencover_to_teamcity_msg.xsl $COVERAGE_FOLDER/coverage/coverage.opencover.xml | grep 'teamcity' ; \
"