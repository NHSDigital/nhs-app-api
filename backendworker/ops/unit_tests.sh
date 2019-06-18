#!/usr/bin/env bash

DOCKER_IMAGE=microsoft/dotnet:2.1-sdk

HOST_PATH=$(pwd)
CONTAINER_PATH=/repo

docker run \
  --rm \
  -v $HOST_PATH:$CONTAINER_PATH $DOCKER_IMAGE \
  /bin/bash -c "  \
    cd /repo ; \
    rm -rf coverage ; \
    apt-get update; \
    apt-get install -y xsltproc; \
    dotnet test /p:CoverletOutputFormat=opencover  /p:CollectCoverage=true /p:CoverletOutput=coverage/ /p:CopyLocalLockFileAssemblies=true; \
    dotnet ~/.nuget/packages/reportgenerator/4.2.0/tools/netcoreapp2.1/ReportGenerator.dll '-reports:**/coverage/coverage.opencover.xml' '-targetdir:coverage' '-reportTypes:HTML;Cobertura' ; \
    mv coverage/index.htm coverage/index.html ; \
    xsltproc ops/cobertura_to_teamcity_msg.xsl coverage/Cobertura.xml | grep 'teamcity' ; \
"