#!/bin/bash
ENV=$(uname -s)
if [[ $ENV =~ ^MING.* ]]
then
echo "Windows"
dir=$(pwd -W)
else
echo "Mac/Linux"
dir=$(pwd)
fi


docker run --rm -v $dir/../../:/repo/ nhsapp.azurecr.io/android:latest bash -c "cd /repo/ ; cd android ; ./gradlew assembleBrowserstacklocal"

dir=$(pwd)

cd $dir/../../bddtests
docker-compose build
cd $dir/../../backendworker
docker-compose build
cd $dir/../../web
docker-compose build
docker-compose -f docker-compose.yml -f docker-compose.browserstacklocal.yml up

