#!/usr/bin/env bash

dotnet test /p:CoverletOutputFormat=opencover  /p:CollectCoverage=true /p:CoverletOutput=coverage/ /p:CopyLocalLockFileAssemblies=true ;
