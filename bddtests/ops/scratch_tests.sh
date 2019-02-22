#!/bin/bash

# set -x
function die () {
    echo >&2 "===]> Error: $@ "
    exit 1
}

function info () {
    echo >&2 "===]> Info: $@ ";
}

#### 1. Login to Azure docker registry (run docker-login.sh script from keybase repo)
#### 2. Then check if your repo names match default ones (if not change them in docker-compose_ci.yml from i.e. `context: ./../nhsonline-web/` to `context: ./../your_name_of_web_repo/`)

DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
BROWSER=${BROWSER:-chromeheadless}
PARALLEL=${PARALLEL:-1}
MODE=${MODE:-local}
TC_CPUS=${TC_CPUS:-3}
TC_RAM=${TC_RAM:-3g}

DOCKER_IMAGE_CHROME=$DOCKER_REGISTRY/chrome:latest
DOCKER_IMAGE_FIREFOX=$DOCKER_REGISTRY/firefox:latest

#### 3. Change browser variable to one webdriver mentioned in ./serenity.properties
if [ -z $BROWSER ]
then
  BROWSER=chromeheadless
fi

#### 4. Select image (with browser inside, it need to match your previous choice)
DOCKER_IMAGE=$DOCKER_IMAGE_CHROME

if [ "$RUN_SUBSET" == 0 ]
then
    info "Test options overridden - User specified Run Configured"
    BDD_CUCUMBER_OPTIONS="$SPECIFIC_TEST_TAGS"
else
    CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
    CURRENT_TAG=$(git name-rev --tags --name-only $CURRENT_BRANCH)
    if [ "$RUN_AS_DEVELOP" == 1 ] || [ $CURRENT_BRANCH == "develop" ] || [ $CURRENT_TAG != "undefined" ]
    then
        info "Main Tranche - Full BDD Test Run Configured"
        BDD_CUCUMBER_OPTIONS="--tags 'not @bug and not @pending and not @manual and not @native and not @tech-debt and not @long-running $SPECIFIC_TEST_TAGS'"
    else
        info "MR Tranche - BDD Smoketest Run Configured"
        BDD_CUCUMBER_OPTIONS="--tags 'not @bug and not @pending and not @manual and not @native and not @tech-debt and
        not @long-running and @smoketest $SPECIFIC_TEST_TAGS'"
    fi
fi

sed -i "s/TEST_ENV/$TEST_ENV/g" vars_stubbed.env

ENV=$(uname -s)

if [[ $ENV =~ ^MING.* ]]
then
  workingDir=$(pwd -W)
else
  workingDir=$(pwd)
fi

info $workingDir

if [ $MODE == "teamcity" ]
then
  docker run \
    --rm \
    --cpus $TC_CPUS \
    --memory $TC_RAM \
    -v $workingDir/../.. \
    --env-file vars_stubbed.env \
    -v $workingDir/../:/repo \
    $DOCKER_IMAGE bash -c " \
      cd /repo ; \
      ./gradlew clean test aggregate --stacktrace\
        -Dcucumber.options=\"$BDD_CUCUMBER_OPTIONS\" \
        -Dwebdriver.provided.type=$BROWSER \
        -Dwebdriver.base.url=$(cat vars_stubbed.env | grep url | cut -f2 -d'=') \
    ;"
else
  docker run \
    --rm \
    --env-file vars_stubbed.env \
    -v $workingDir/../:/repo \
    $DOCKER_IMAGE bash -c " \
      cd /repo ; \
      ./gradlew clean test aggregate --stacktrace\
        -Dcucumber.options=\"$BDD_CUCUMBER_OPTIONS\" \
        -Dwebdriver.provided.type=$BROWSER \
        -Dwebdriver.base.url=$(cat vars_stubbed.env | grep url | cut -f2 -d'=') \
    ;"
fi

test_exit_code=$?

mkdir -p logs
for container in $(docker-compose -f docker-compose_ci.yml ps | tail -n +3 | awk '{print $1}' ); do
  docker logs $container >& ./logs/$container.log
done

sed -i "s/TEST_ENV/$TEST_ENV/g" vars_stubbed.env

exit "$test_exit_code"
