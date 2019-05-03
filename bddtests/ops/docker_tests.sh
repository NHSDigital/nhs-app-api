#!/bin/bash

# set -x
function die () {
    echo >&2 "===]> Error: $@ "
    exit 1
}

function info () {
    echo >&2 "===]> Info: $@ ";
}

info "Output currently executing docker containers to aid debug"
docker ps

info "Cleaning up hanging containers"
docker kill $(docker ps | grep -v clair | grep -v CONTAINER | awk '{print $1}') 2>/dev/null

# Check if DOCKER_TAG exists in envvar
[ -z $APP_DOCKER_TAG ] && die "APP_DOCKER_TAG is not specified, it should be so we can pin builds to a specific version rather than latest"

#### 1. First login to azure docker registry (you can do it by running docker-login.sh script from keybase repo)
#### 2. Then check if your repo names match default ones (if not change them in docker-compose_ci.yml from i.e. `context: ./../nhsonline-web/` to `context: ./../your_name_of_web_repo/`)

DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
BROWSER=${BROWSER:-chromeheadless}
PARALLEL=${PARALLEL:-0}
MODE=${MODE:-local}
TC_CPUS=${TC_CPUS:-3}
TC_RAM=${TC_RAM:-3g}
MAX_TESTTHREADS=${MAX_TESTTHREADS:-8}
ACCESSIBILITY_OUTPUT=${ACCESSIBILITY_OUTPUT_FOLDER:-accessibilityoutput}

# Free up some docker space if on TC

if [ $MODE == "teamcity" ]
then
  info "Cleaning up docker networks"
  docker network prune -f

  info "Cleaning docker volumes"
  docker volume prune -f

  info "Cleaning docker system"
  docker system prune -f -a
fi

DOCKER_IMAGE_CHROME=$DOCKER_REGISTRY/chrome:latest
DOCKER_IMAGE_FIREFOX=$DOCKER_REGISTRY/firefox:latest
DOCKER_IMAGE_BROWSERSTACK=$DOCKER_REGISTRY/browserstack:latest

#### 3. Change browser variable to one webdriver mentioned in ./serenity.properties

# create run time version of docker-compose and environment variables so we can amend values for throttling/cosmos tests
cp docker-compose_ci.yml docker-compose_ci_run.yml
cp vars_ci.env vars_ci_run.env
sed -i '' -e 's/vars_ci.env/vars_ci_run.env/g' docker-compose_ci_run.yml

# List all docker images in the docker compose setup
DOCKER_SERVICES=`docker-compose -f docker-compose_ci_run.yml config --services`

if [ "$RUN_SUBSET" == 0 ]
then
    info "Test options overridden - User specified Run Configured"
    BDD_CUCUMBER_OPTIONS_PREFIX="$SPECIFIC_TEST_TAGS"
    TAGS=specific
else
    CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
    CURRENT_TAG=$(git name-rev --tags --name-only $CURRENT_BRANCH)
    if [ "$RUN_AS_DEVELOP" == 1 ] || [ $CURRENT_BRANCH == "develop" ] || [ $CURRENT_TAG != "undefined" ]
    then
        if [ "$ENABLE_LONG_RUNNING" == 1 ]
        then
          info "Main Tranche - Full BDD Test including Long Running Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @bug and not @pending and not @manual and not @native and not
          @tech-debt and not @throttling and not @cosmos and not @accessibility"
        elif [ "$RUN_NATIVE" == 1 ] && [ "$BROWSER" == "browserstack_ios" ]
        then
          info "Main Tranche - Full BDD Test including Long Running Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @nativepending and not
          @nativebug and not @backend and not @bug and not @pending and not @manual and not @tech-debt and not
          @throttling and not @cosmos and not @noJs and not @android and not @accessibility"
        elif [ "$RUN_NATIVE" == 1 ] && [ "$BROWSER" == "browserstack_android" ]
        then
          info "Main Tranche - Full BDD Test including Long Running Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @nativepending and not
          @nativebug and not @backend and not @bug and not @pending and not @manual and not @tech-debt and not
          @throttling and not @cosmos and not @noJs and not @ios and not @accessibility"
        else
          info "Main Tranche - Full BDD Test Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @bug and not @pending and not @manual and not @native and not
          @tech-debt and not @long-running and not @throttling and not
          @cosmos and not @accessibility $SPECIFIC_TEST_TAGS"
        fi
        if [ "$PARALLEL" == 1 ] && [ "$RUN_NATIVE" != 1 ] &&  [ $MODE == "teamcity" ]
        then
          TAGS=()
          val=1
          for filename in $(find .. | grep -F .feature | grep -v throttling.feature); do

            info "Tagging $filename as tranche$val"

            echo -e "@tranche$val\n$(cat $filename)" > $filename

            TAGS+=(tranche$val)

            let "val +=1"
          done

          TAGS+=(throttling)
          
        elif [ "$RUN_NATIVE" == 1 ]
        then
          TAGS=(native-smoketest)
        else
          TAGS=specific
          BDD_CUCUMBER_OPTIONS_PREFIX=$BDD_CUCUMBER_OPTIONS_PREFIX"'"
        fi
    elif [ "$ENABLE_COSMOS_TESTS" == 1 ]
    then
        [ -z "$COSMOS_AUTHKEY" ] && die "COSMOS_AUTHKEY not specified, it is required if cosmos tests are enabled"
        echo "TERMS_CONDITIONS_COSMOS_AUTH_KEY=$COSMOS_AUTHKEY" >> vars_ci_run.env
        sed -i '' -e 's/STUB\_TERMS\_AND\_CONDITIONS\=true/STUB\_TERMS\_AND\_CONDITIONS\=false/g' vars_ci_run.env
        info "Main Tranche - Run Cosmos Tests"
        BDD_CUCUMBER_OPTIONS_PREFIX="--tags '@cosmos'"
        TAGS=(specific)
    else
        info "MR Tranche - BDD Smoketest Run Configured"
        BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @bug and not @pending and not @manual and not @native and not @tech-debt and
        not @long-running and not @throttling and not @cosmos and not @accessibility $SPECIFIC_TEST_TAGS"
        TAGS=smoketest
    fi
fi

info $BDD_CUCUMBER_OPTIONS_PREFIX

# Pin versions of docker images
export WEB_TAG=$APP_DOCKER_TAG
export BACKEND_TAG=$APP_DOCKER_TAG
[ -z $REDIS_DATA_DOCKER_TAG ] || export REDIS_DATA_TAG=$REDIS_DATA_DOCKER_TAG

# Change an image to appropriate one (with proper browser inside, it needs to match your previous choice :D)
if [ "$BROWSER" == "browserstack_android" ] || [ "$BROWSER" == "browserstack_ios" ]
then
  DOCKER_IMAGE=$DOCKER_IMAGE_BROWSERSTACK
  AUTOLOGIN="AUTOLOGIN=true"
  APPSCHEME="APP_SCHEME=nhsapp"
  if [ "$BROWSER" == "browserstack_android" ]
  then
    APPIUM_TYPE="-Dappium.platformName=ANDROID"
    if [ -z "$DEVICE" ] && [ -z "$OS" ]
    then
        DEVICENAME="BROWSERSTACK_DEVICE_NAME=$DEVICE"
        OSVERSION="BROWSERSTACK_OS_VERSION=$OS"
    else
        DEVICENAME="BROWSERSTACK_DEVICE_NAME=Google\ Pixel\ 2"
        OSVERSION="BROWSERSTACK_OS_VERSION=8.0"
    fi
  else
    APPIUM_TYPE="-Dappium.platformName=iOS"
    if [ -z "$DEVICE" ] && [ -z "$OS" ]
    then
        DEVICENAME="BROWSERSTACK_DEVICE_NAME=$DEVICE"
        OSVERSION="BROWSERSTACK_OS_VERSION=$OS"
    else
        DEVICENAME="BROWSERSTACK_DEVICE_NAME=iPhone\ 8"
        OSVERSION="BROWSERSTACK_OS_VERSION=12.1"
    fi
  fi
else
  DOCKER_IMAGE=$DOCKER_IMAGE_CHROME
fi


# Pull images
docker pull $DOCKER_IMAGE

ENV=$(uname -s)

if [[ $ENV =~ ^MING.* ]]
then
  workingDir=$(pwd -W)
else
  workingDir=$(pwd)
fi

info $workingDir

# Prepare for Run
if [ $MODE == "teamcity" ]
then
docker run \
  --cpus $TC_CPUS \
  --memory $TC_RAM \
  --rm \
  -v $workingDir/../:/repo \
  $DOCKER_IMAGE bash -c " \
    cd /repo ; \
    ./gradlew clean prepare"
else
docker run \
  --rm \
  -v $workingDir/../:/repo \
  $DOCKER_IMAGE bash -c " \
    cd /repo ; \
    ./gradlew clean prepare"
fi

test_exit_code=$?

if [ $test_exit_code != 0 ]
then
  exit $test_exit_code
fi

for s in $DOCKER_SERVICES; do
  #Don't pull local images we've built as part of the pipeline
  if [[ "$s" != "api.local.bitraft.io" && "$s" != "web.local.bitraft.io" && "$s" != "nhsonline-backendservicejourneyrulesapi" && "$s" != "nhsonline-backendcidapi" ]]; then 
    docker-compose -f docker-compose_ci_run.yml pull $s
  fi
done

# Output list of images contained in config
docker-compose -f docker-compose_ci_run.yml config | grep image

#pre-cleanup
rm -rf $workingDir/../../testRunFolder/*

for TAG in ${TAGS[*]}; do
  info "Creating test folder for $TAG"

  cp -r $workingDir/../ $workingDir/../../testRunFolder/$TAG

done

PIDS=()

for TAG in ${TAGS[*]}; do

info "Running $TAG tests"

  if [ $TAG == "throttling" ]
  then
    sed -i '' -e 's/THROTTLING\_ENABLED\=false/THROTTLING\_ENABLED\=true/g' vars_ci_run.env
  else
    sed -i '' -e 's/THROTTLING\_ENABLED\=true/THROTTLING\_ENABLED\=false/g' vars_ci_run.env
  fi

  # Run docker tests per tag
  docker-compose -p $TAG -f docker-compose_ci_run.yml up -d --build || die "Docker compose failure"

  ##################### Runtime vars
  WEB_ID=$(docker ps -qf name=${TAG}_web.*)
  NETWORK=$(docker inspect $WEB_ID --format '{{range .NetworkSettings.Networks}}{{.NetworkID}}{{end}}' | cut -c 1-12)
  #####################

  if [ $TAG == "specific" ]
  then
    BDD_CUCUMBER_OPTIONS=$BDD_CUCUMBER_OPTIONS_PREFIX
  else
    if [ $TAG == "other" ]
      then
        BDD_CUCUMBER_OPTIONS="--strict $BDD_CUCUMBER_OPTIONS_PREFIX"
        for TESTTAG in ${TAGS[*]}; do
          if [ $TESTTAG != "other" ]
          then
            BDD_CUCUMBER_OPTIONS+=" and not @$TESTTAG"
          fi
        done
        BDD_CUCUMBER_OPTIONS+="'"
    else
      if [ $TAG == "throttling" ]
      then
        BDD_CUCUMBER_OPTIONS="--strict --tags '@$TAG and not @native'"
      else
        BDD_CUCUMBER_OPTIONS="--strict $BDD_CUCUMBER_OPTIONS_PREFIX and @$TAG'"
      fi
    fi
  fi

  info $BDD_CUCUMBER_OPTIONS

  if [ $DOCKER_IMAGE == $DOCKER_IMAGE_BROWSERSTACK ]
  then
      BROWSERSTACK_LOCAL_STRING="(BrowserStackLocal --key $BROWSERSTACK_ACCESSKEY --force-local --local-identifier $NETWORK &) ;"
  fi

  if [ $MODE == "teamcity" ]
  then
    docker run \
      --name $TAG \
      --rm \
      --network $NETWORK \
      --env-file vars_ci_run.env \
      --cpus $TC_CPUS \
      --memory $TC_RAM \
      -v $workingDir/../../testRunFolder/$TAG/:/repo \
      $DOCKER_IMAGE bash -c " \
        echo $(DATE) - $TAG Starting
        cd /repo ; \
        $BROWSERSTACK_LOCAL_STRING \
        BROWSERSTACK_ACCESSKEY=$BROWSERSTACK_ACCESSKEY BROWSERSTACK_USERNAME=$BROWSERSTACK_USERNAME \
        APP_PATH=$BROWSERSTACK_APPPATH BROWSERSTACK_LOCAL_IDENTIFIER=$NETWORK $DEVICENAME $OSVERSION $APPSCHEME $AUTOLOGIN \
        ./gradlew test --stacktrace \
          -Dcucumber.options=\"--strict $BDD_CUCUMBER_OPTIONS \" \
          -Dwebdriver.provided.type=$BROWSER \
          $APPIUM_TYPE \
          -Dwebdriver.base.url=$(cat vars_ci_run.env | grep url | cut -f2 -d'=') ; echo $(DATE) - $TAG Completed" &
  else
    docker run \
      --name $TAG \
      --rm \
      --network $NETWORK \
      --env-file vars_ci_run.env \
      -v $workingDir/../../testRunFolder/$TAG/:/repo \
      $DOCKER_IMAGE bash -c " \
        cd /repo ; \
        $BROWSERSTACK_LOCAL_STRING \
        BROWSERSTACK_ACCESSKEY=$BROWSERSTACK_ACCESSKEY BROWSERSTACK_USERNAME=$BROWSERSTACK_USERNAME \
        APP_PATH=$BROWSERSTACK_APPPATH BROWSERSTACK_LOCAL_IDENTIFIER=$NETWORK $DEVICENAME $OSVERSION $APPSCHEME $AUTOLOGIN \
        ./gradlew test --stacktrace \
          -Dcucumber.options=\"--strict $BDD_CUCUMBER_OPTIONS \" \
          -Dwebdriver.provided.type=$BROWSER \
          $APPIUM_TYPE \
          -Dwebdriver.base.url=$(cat vars_ci_run.env | grep url | cut -f2 -d'=')" &
  fi

  PID=$!

  if [ "$PARALLEL" == 1 ] && [ "$RUN_NATIVE" != 1 ] &&  [ $MODE == "teamcity" ]
  then
    while [ $MAX_TESTTHREADS -le $(docker ps | grep -v local.bitraft | grep -v nhsonline | wc -l) ]
    do
      echo "Max threads reached - sleeping"
      sleep 10
    done

    #clean up any open container groups
    CONTAINERS=$(docker ps | grep web.local.bitraft.io | awk '{print $1}')

    for CONTAINER in ${CONTAINERS[*]}; do
      TAG=$(docker ps --format '{{.ID}} {{.Label "com.docker.compose.project"}}' | grep $CONTAINER | awk '{print $2}')

      if [ $(docker ps | grep $TAG | grep -v local.bitraft | grep -v nhsonline | wc -l) == 0 ]
      then
        info "Shutting down $TAG"
        docker-compose -p $TAG -f docker-compose_ci_run.yml stop
        docker network prune -f
        docker volume prune -f
      fi
    done
  else  
    wait $PID
  fi

  PIDS+=($PID)

done

# Wait for all test runners
info "Waiting for all processes to terminate"
for PID in ${PIDS[*]}; do
    wait $PID
done

# Aggregate test results
info "Aggregating test results"
for TAG in ${TAGS[*]}; do
  cp -r $workingDir/../../testRunFolder/$TAG/target/site/serenity $workingDir/../target/site/.
  cp -r $workingDir/../../testRunFolder/$TAG/build/test-results $workingDir/../build/.
  cp -r $workingDir/../../testRunFolder/$TAG/$ACCESSIBILITY_OUTPUT $workingDir/../.
done

if [ $MODE == "teamcity" ]
then
docker run \
  --cpus $TC_CPUS \
  --memory $TC_RAM \
  --rm \
  -v $workingDir/../:/repo \
  $DOCKER_IMAGE bash -c " \
    cd /repo ; \
    ./gradlew aggregate"
else
docker run \
  --rm \
  -v $workingDir/../:/repo \
  $DOCKER_IMAGE bash -c " \
    cd /repo ; \
    ./gradlew aggregate"
fi

test_exit_code=$?

mkdir -p logs

for TAG in ${TAGS[*]}; do

  for container in $(docker-compose -p ${TAG} -f docker-compose_ci_run.yml ps | tail -n +3 | awk '{print $1}' ); do
    docker logs $container >& ./logs/$container.log
  done

  docker-compose -p $TAG -f docker-compose_ci_run.yml stop

done

#cleanup
rm -rf $workingDir/../../testRunFolder/*
rm -rf docker-compose_ci_run.yml
rm -rf fars_ci_run.env

docker rm -f $(docker ps -aq)

exit "$test_exit_code"
