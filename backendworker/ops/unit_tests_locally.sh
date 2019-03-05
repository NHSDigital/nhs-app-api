#!/usr/bin/env bash

UNIT_TESTS=(NHSOnline.Backend.PfsApi.UnitTests NHSOnline.Backend.CidApi.UnitTests NHSOnline.Backend.GpSystems.UnitTests NHSOnline.Backend.Support.UnitTests NHSOnline.Backend.ApiSupport.UnitTests)

for test in "${UNIT_TESTS[@]}"
do
  dotnet test /p:CoverletOutputFormat=opencover  /p:CollectCoverage=true /p:CoverletOutput=coverage/ $test ;
done
