#!/usr/bin/env bash

DOCKER_IMAGE=microsoft/dotnet:2.1-sdk

HOST_PATH=$(pwd)
CONTAINER_PATH=/repo

docker run \
  --rm \
  -v $HOST_PATH:$CONTAINER_PATH $DOCKER_IMAGE \
  /bin/bash -c "  \
  cd /repo ; \
  rm -rf NHSOnline.Backend.Worker.UnitTests/coverage ; \
  apt-get update; \
  apt-get install -y xsltproc; \
  dotnet test /p:CoverletOutputFormat=opencover  /p:CollectCoverage=true /p:CoverletOutput=coverage/ NHSOnline.Backend.Worker.UnitTests ; \
  dotnet ~/.nuget/packages/reportgenerator/4.0.2/tools/netcoreapp2.0/ReportGenerator.dll '-reports:NHSOnline.Backend.Worker.UnitTests/coverage/coverage.opencover.xml' '-targetdir:NHSOnline.Backend.Worker.UnitTests/coverage' ; \
  mv NHSOnline.Backend.Worker.UnitTests/coverage/index.htm NHSOnline.Backend.Worker.UnitTests/coverage/index.html ; \
  xsltproc NHSOnline.Backend.Worker.UnitTests/opencover_to_teamcity_msg.xsl NHSOnline.Backend.Worker.UnitTests/coverage/coverage.opencover.xml | grep 'teamcity' ; \
"
