#!/bin/bash

# set -x
set -e

function die () {
    echo >&2 "===]> Error: $@ "
    exit 1
}

function info () {
    echo >&2 "===]> Info: $@ ";
}

# Check if APP_DOCKER_TAG exists in envvar
[ -z $APP_DOCKER_TAG ] && die "APP_DOCKER_TAG is not specified, it should be so we can pin builds to a specific version rather than latest"

if [ ! -f ~/.nhsonline/secrets/azure_notification_hub_key_bdd ]; then
  [ -z $AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY ] && die "AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY is not specified, this is required to run notifications BDD tests"
  mkdir -p ~/.nhsonline/secrets
  echo "$AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" >> ~/.nhsonline/secrets/azure_notification_hub_key_bdd
fi

if [ "$RUN_NATIVE" == 1 ]; then
  if [ -z $BROWSERSTACK_ACCESSKEY ] && [ -f ~/.nhsonline/secrets/browserstack_accesskey ]; then
    export BROWSERSTACK_ACCESSKEY=$(<~/.nhsonline/secrets/browserstack_accesskey)
  fi

  [ -z $BROWSERSTACK_ACCESSKEY ] && die "BROWSERSTACK_ACCESSKEY is not specified, this is required to run native BDD tests"
  [ -z $BROWSERSTACK_APPPATH ] && die "BROWSERSTACK_APPPATH is not specified, this is required to run native BDD tests"
  [ "$BROWSER" != "browserstack_ios" ] && [ "$BROWSER" != "browserstack_android" ] && die "BROWSER must be browserstack_ios or browserstack_android to run native BDD tests"

  BROWSERSTACK_USERNAME=${BROWSERSTACK_USERNAME:-ops20}
fi

# Support TeamCity running with APP_DOCKER_TAG
export WEB_DOCKER_TAG=${WEB_DOCKER_TAG:-$APP_DOCKER_TAG}
export WEB_DOCKER_REGISTRY=${WEB_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export PFSAPI_DOCKER_TAG=${PFSAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export PFSAPI_DOCKER_REGISTRY=${PFSAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export CIDAPI_DOCKER_TAG=${CIDAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export CIDAPI_DOCKER_REGISTRY=${CIDAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export SJRCONFIG_DOCKER_TAG=${SJRCONFIG_DOCKER_TAG:-$APP_DOCKER_TAG}
export SJRCONFIG_DOCKER_REGISTRY=${SJRCONFIG_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export SJRAPI_DOCKER_TAG=${SJRAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export SJRAPI_DOCKER_REGISTRY=${SJRAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export USERSAPI_DOCKER_TAG=${USERSAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export USERSAPI_DOCKER_REGISTRY=${USERSAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export USERINFOAPI_DOCKER_TAG=${USERINFOAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export USERINFOAPI_DOCKER_REGISTRY=${USERINFOAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export LOGAPI_DOCKER_TAG=${LOGAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export LOGAPI_DOCKER_REGISTRY=${LOGAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}
export MESSAGESAPI_DOCKER_TAG=${MESSAGESAPI_DOCKER_TAG:-$APP_DOCKER_TAG}
export MESSAGESAPI_DOCKER_REGISTRY=${MESSAGESAPI_DOCKER_REGISTRY:-nhsapp.azurecr.io}

if [ -z "$TF_BUILD" ] # Azure DevOps creates clean build agents so no need to clean
then
  info "Output currently executing docker containers to aid debug"
  docker ps

  info "Cleaning up hanging containers"
  HANGING_CONTAINERS=$(docker ps | grep -v clair | grep -v CONTAINER | awk '{print $1}')
  [ -z "$HANGING_CONTAINERS" ] || docker kill $HANGING_CONTAINERS
fi

# create run time version of docker-compose and environment variables so we can append values for throttling/cosmos tests
DOCKER_COMPOSE_FILES="../../docker-compose.yml ../docker-compose.yml"
if [ "$RUN_LOCAL_BDD" == 1 ]
then
  DOCKER_COMPOSE_PORT_FILES=$(env | grep _DOCKER_PORTS | sed 's#^.*_DOCKER_PORTS=#../../#')
  DOCKER_COMPOSE_FILES="$DOCKER_COMPOSE_FILES ../../docker-compose.ports.yml ../docker-compose.ports.yml $DOCKER_COMPOSE_PORT_FILES"
fi

echo 'version: "3.4"' > docker-compose.ci-run.yml
echo 'services:' >> docker-compose.ci-run.yml
for SERVICE in `docker-compose -f $(echo $DOCKER_COMPOSE_FILES | sed "s/ / -f /g") config --services`; do
SERVICE="$(echo -e "${SERVICE}" | tr -d '[:space:]')"
  echo "  ${SERVICE}:" >> docker-compose.ci-run.yml
  echo "    env_file:" >> docker-compose.ci-run.yml
  echo "      - ./bddtests/ops/vars_ci_run.env" >> docker-compose.ci-run.yml
done

DOCKER_COMPOSE_FILES="$DOCKER_COMPOSE_FILES docker-compose.ci-run.yml"
DOCKER_COMPOSE_FILES_ARG="-f $(echo $DOCKER_COMPOSE_FILES | sed "s/ / -f /g")"

echo "VERSION_TAG=dev_bdd_docker_ops_ci" > vars_ci_run.env

DOCKER_REGISTRY=${DOCKER_REGISTRY:-nhsapp.azurecr.io}
BROWSER=${BROWSER:-chromeheadless}
PARALLEL=${PARALLEL:-0}
MODE=${MODE:-local}
TC_CPUS=${TC_CPUS:-3}
TC_RAM=${TC_RAM:-3g}
MAX_TESTTHREADS=${MAX_TESTTHREADS:-8}
ACCESSIBILITY_OUTPUT=${ACCESSIBILITY_OUTPUT_FOLDER:-accessibilityoutput}
IOSURLSUFFIX="?source=ios"
ADROIDURLSUFFIX="?source=android"
XSLTPROC_DOCKER_IMAGE=hinesteve/xsltproc
USER_ID=`id -u`
GROUP_ID=`id -g`

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

if [ "$RUN_SUBSET" == 0 ]
then
    info "Test options overridden - User specified Run Configured"
    BDD_CUCUMBER_OPTIONS_PREFIX="$SPECIFIC_TEST_TAGS"
    TAGS=(specific)
else
    CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
    CURRENT_TAG=$(git name-rev --tags --name-only $CURRENT_BRANCH)
    if [ "$RUN_AS_DEVELOP" == 1 ] || [ $CURRENT_BRANCH == "develop" ] || [ $CURRENT_TAG != "undefined" ]
    then
        if [ "$ENABLE_LONG_RUNNING" == 1 ]
        then
          info "Main Tranche - Full BDD Test including Long Running Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @bug and not @pending and not @manual and not @native and not
          @tech-debt and not @throttling and not @cosmos and not @accessibility and not
          @onlineconsultations"
        elif [ "$RUN_NATIVE" == 1 ] && [ "$BROWSER" == "browserstack_ios" ]
        then
          info "Main Tranche - Full iOS BDD Test including Long Running Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @nativepending and not
          @nativebug and not @backend and not @bug and not @pending and not @manual and not @tech-debt and not
          @throttling and not @cosmos and not @noJs and not @android and not @accessibility and not
          @onlineconsultations"
        elif [ "$RUN_NATIVE" == 1 ] && [ "$BROWSER" == "browserstack_android" ]
        then
          info "Main Tranche - Full Android BDD Test including Long Running Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @nativepending and not
          @nativebug and not @backend and not @bug and not @pending and not @manual and not @tech-debt and not
          @throttling and not @cosmos and not @noJs and not @ios and not @accessibility and not @onlineconsultations"
        else
          info "Main Tranche - Full BDD Test Run Configured"
          BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @bug and not @pending and not @manual and not @native and not
          @tech-debt and not @long-running and not @throttling and not
          @cosmos and not @accessibility and not @onlineconsultations $SPECIFIC_TEST_TAGS"
        fi
        if [ "$PARALLEL" == 1 ] && [ "$RUN_NATIVE" != 1 ]
        then
          TAGS=()
          val=1
          for filename in $(find .. | grep -F .feature | grep -v throttling.feature | sort); do
            let tranche="$(( val++ % MAX_TESTTHREADS + 1))"

            info "Tagging $filename as tranche$tranche"
            echo -e "@tranche$tranche\n$(cat $filename)" > $filename

            TAGS+=(tranche$tranche)
          done

          TAGS=($(tr ' ' '\n' <<< "${TAGS[@]}" | sort -u | tr '\n' ' '))
          TAGS+=(throttling)
          TAGS+=(onlineconsultations)

        elif [ "$RUN_NATIVE" == 1 ]
        then
          TAGS=(nativesmoketest)
        else
          TAGS=(specific)
          BDD_CUCUMBER_OPTIONS_PREFIX=$BDD_CUCUMBER_OPTIONS_PREFIX"'"
        fi
    elif [ "$ENABLE_COSMOS_TESTS" == 1 ]
    then
        [ -z "$COSMOS_AUTHKEY" ] && die "COSMOS_AUTHKEY not specified, it is required if cosmos tests are enabled"
        echo "TERMS_CONDITIONS_COSMOS_AUTH_KEY=$COSMOS_AUTHKEY" >> vars_ci_run.env
        echo 'STUB_TERMS_AND_CONDITIONS=false' >> vars_ci_run.env
        info "Main Tranche - Run Cosmos Tests"
        BDD_CUCUMBER_OPTIONS_PREFIX="--tags '@cosmos'"
        TAGS=(specific)
    else
        info "MR Tranche - BDD Smoketest Run Configured"
        BDD_CUCUMBER_OPTIONS_PREFIX="--tags 'not @bug and not @pending and not @manual and not @native and not @tech-debt and
        not @long-running and not @throttling and not @cosmos and not @accessibility and not
        @onlineconsultations $SPECIFIC_TEST_TAGS"
        TAGS=(smoketest)
    fi
fi

info $BDD_CUCUMBER_OPTIONS_PREFIX
info ${TAGS[*]}

for TAG in ${TAGS[*]}; do
  LAST_TAG=$TAG
done

# Change an image to appropriate one (with proper browser inside, it needs to match your previous choice :D)
if [ "$BROWSER" == "browserstack_android" ] || [ "$BROWSER" == "browserstack_ios" ]
then
  DOCKER_IMAGE=$DOCKER_IMAGE_BROWSERSTACK
  AUTOLOGIN="AUTOLOGIN=true"
  APPSCHEME="APP_SCHEME=nhsapp"
  if [ "$BROWSER" == "browserstack_android" ]
  then
    APPIUM_TYPE="-Dappium.platformName=ANDROID"
    URLSUFFIX="URL_NATIVE_SUFFIX=$ADROIDURLSUFFIX"
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
    URLSUFFIX="URL_NATIVE_SUFFIX=$IOSURLSUFFIX"
    if [ -z "$DEVICE" ] && [ -z "$OS" ]
    then
        DEVICENAME="BROWSERSTACK_DEVICE_NAME=$DEVICE"
        OSVERSION="BROWSERSTACK_OS_VERSION=$OS"

    else
        DEVICENAME="BROWSERSTACK_DEVICE_NAME=\"iPhone 8\""
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

info "Working Directory: $workingDir"

# Prepare for Run
if [ "$SKIP_PREPARE" != 1 ]
then
  if [ $MODE == "teamcity" ]
  then
    docker run \
    --cpus $TC_CPUS \
    --memory $TC_RAM \
    --rm \
    -v $workingDir/../:/repo \
    $DOCKER_IMAGE bash -c " \
      set -e ; \
      cd /repo ; \
      ./gradlew --no-daemon clean prepare; \
      chown -R $USER_ID:$GROUP_ID /repo"
  else
    docker run \
    --rm \
    -v $workingDir/../:/repo \
    $DOCKER_IMAGE bash -c " \
      set -e ; \
      cd /repo ; \
      ./gradlew --no-daemon clean prepare; \
      chown -R $USER_ID:$GROUP_ID /repo"
  fi
fi

DOCKER_IMAGES=$(docker-compose $DOCKER_COMPOSE_FILES_ARG config | grep image | sed -e 's/^[[:space:]]*image: //' | sort -u)
info "Configured images:"
info $DOCKER_IMAGES

if [ -z "$NO_PULL" ]; then
  # Don't try to pull local images
  DOCKER_IMAGES_TO_PULL=$(echo $DOCKER_IMAGES | sed -e 's#local/[^[:space:]]\+##g' -e 's/ \+/ /' | sort -u)
  for IMAGE_TO_PULL in $DOCKER_IMAGES_TO_PULL; do
    docker pull $IMAGE_TO_PULL
  done
fi

TEST_RUN_FOLDER="$workingDir/../../testRunFolder"
if [ -e $TEST_RUN_FOLDER ]
then
  rm -rf $TEST_RUN_FOLDER
fi
mkdir -p $TEST_RUN_FOLDER

for TAG in ${TAGS[*]}; do
  info "Creating test folder for $TAG"

  cp -r $workingDir/../ $TEST_RUN_FOLDER/$TAG
done

PIDS=()

for TAG in ${TAGS[*]}; do

  info "Running $TAG tests"

  if [ $TAG == "throttling" ]
  then
    echo 'THROTTLING_ENABLED=true' >> vars_ci_run.env
  else
    echo 'THROTTLING_ENABLED=false' >> vars_ci_run.env
  fi

  if [ $TAG == "onlineconsultations" ]
  then
    echo 'ONLINE_CONSULTATIONS_ENABLED=true' >> vars_ci_run.env
  else
    echo 'ONLINE_CONSULTATIONS_ENABLED=false' >> vars_ci_run.env
  fi

  if [ $TAG == "nativesmoketest" ]
  then
    echo 'ConfigurationSettings__DefaultSessionExpiryMinutes=5' >> vars_ci_run.env
    echo 'EXPIRE_AFTER_SECONDS=300' >> vars_ci_run.env
    echo 'SESSION_EXPIRY_MINUTES=5' >> vars_ci_run.env
    echo 'ONLINE_CONSULTATIONS_ENABLED=false' >> vars_ci_run.env
  else
    echo 'ConfigurationSettings__DefaultSessionExpiryMinutes=2' >> vars_ci_run.env
    echo 'EXPIRE_AFTER_SECONDS=120' >> vars_ci_run.env
    echo 'SESSION_EXPIRY_MINUTES=2' >> vars_ci_run.env
  fi

  # Run docker tests per tag
  docker-compose -p $TAG $DOCKER_COMPOSE_FILES_ARG up -d --build || die "Docker compose failure"

  if [ "$RUN_LOCAL_BDD" == 1 ]
  then
    echo docker-compose -p $TAG $DOCKER_COMPOSE_FILES_ARG down
    exit
  fi

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
        BDD_CUCUMBER_OPTIONS="$BDD_CUCUMBER_OPTIONS_PREFIX"
        for TESTTAG in ${TAGS[*]}; do
          if [ $TESTTAG != "other" ]
          then
            BDD_CUCUMBER_OPTIONS+=" and not @$TESTTAG"
          fi
        done
        BDD_CUCUMBER_OPTIONS+="'"
    else
      if [ $TAG == "throttling" ] || [ $TAG == "onlineconsultations" ]
      then
        BDD_CUCUMBER_OPTIONS="--strict --tags '@$TAG and not @native'"
      else
        BDD_CUCUMBER_OPTIONS="$BDD_CUCUMBER_OPTIONS_PREFIX and @$TAG'"
      fi
    fi
  fi

  info "BDD Cucumber Option: $BDD_CUCUMBER_OPTIONS"

  if [ "$DOCKER_IMAGE" == "$DOCKER_IMAGE_BROWSERSTACK" ]
  then
      BROWSERSTACK_LOCAL_STRING="(BrowserStackLocal --key $BROWSERSTACK_ACCESSKEY --force-local --local-identifier $NETWORK &) ;"
      info BROWSERSTACK_LOCAL_STRING
  fi

  if [ $MODE == "teamcity" ]
  then
    docker run \
      --name $TAG \
      --rm \
      --network $NETWORK \
      --env-file vars_test_runner.env \
      --env-file vars_ci_run.env \
      --cpus $TC_CPUS \
      --memory $TC_RAM \
      -v $TEST_RUN_FOLDER/$TAG/:/repo \
      $DOCKER_IMAGE bash -c " \
        echo $(date) - $TAG Starting \
        set -e ; \
        cd /repo ; \
        $BROWSERSTACK_LOCAL_STRING \
        BROWSERSTACK_ACCESSKEY=$BROWSERSTACK_ACCESSKEY BROWSERSTACK_USERNAME=$BROWSERSTACK_USERNAME $URLSUFFIX\
        APP_PATH=$BROWSERSTACK_APPPATH BROWSERSTACK_LOCAL_IDENTIFIER=$NETWORK $DEVICENAME $OSVERSION $APPSCHEME $AUTOLOGIN \
        ./gradlew test --no-daemon --stacktrace \
          -Dcucumber.options=\"--strict $BDD_CUCUMBER_OPTIONS \" \
          -Dwebdriver.provided.type=$BROWSER \
          $APPIUM_TYPE \
          -Dwebdriver.base.url=$(cat vars_test_runner.env | grep url | cut -f2 -d'=') ; \
          echo $(date) - $TAG Completed; \
        chown -R $USER_ID:$GROUP_ID /repo" &
      
    PID=$!

    # give the test container time to startup
    echo "Sleeping for 10 seconds to allow the test container to start"
    sleep 10

  else
    docker run \
      --name $TAG \
      --rm \
      --network $NETWORK \
      --env-file vars_test_runner.env \
      --env-file vars_ci_run.env \
      -v $TEST_RUN_FOLDER/$TAG/:/repo \
      $DOCKER_IMAGE bash -c " \
        set -e ; \
        cd /repo ; \
        $BROWSERSTACK_LOCAL_STRING \
        BROWSERSTACK_ACCESSKEY=$BROWSERSTACK_ACCESSKEY BROWSERSTACK_USERNAME=$BROWSERSTACK_USERNAME $URLSUFFIX\
        APP_PATH=$BROWSERSTACK_APPPATH BROWSERSTACK_LOCAL_IDENTIFIER=$NETWORK $DEVICENAME $OSVERSION $APPSCHEME $AUTOLOGIN \
        ./gradlew test --no-daemon --stacktrace \
          -Dcucumber.options=\"--strict $BDD_CUCUMBER_OPTIONS \" \
          -Dwebdriver.provided.type=$BROWSER \
          $APPIUM_TYPE \
          -Dwebdriver.base.url=$(cat vars_test_runner.env | grep url | cut -f2 -d'='); \
        chown -R $USER_ID:$GROUP_ID /repo" &
      
      PID=$!
  fi

  if [ "$PARALLEL" == 1 ] && [ "$RUN_NATIVE" != 1 ]
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

      # Always leave 'LAST_TAG' running in case of re-run
      if [ "$TAG" != "$LAST_TAG" ] && [ $(docker ps | grep $TAG | grep -v local.bitraft | grep -v nhsonline | wc -l) == 0 ]
      then
        info "Shutting down $TAG"
        docker-compose -p $TAG $DOCKER_COMPOSE_FILES_ARG stop
        docker network prune -f
        docker volume prune -f
      fi
    done
  else
    wait $PID
    info "$PID is finished"
  fi

  PIDS+=($PID)

done

set +e

# Wait for all test runners
info "Waiting for all processes to terminate"
for PID in ${PIDS[*]}; do
    wait $PID
done

set -e

info "Collecting failed test reports"
touch $workingDir/../build/failures.txt
for TAG in ${TAGS[*]}; do
  info "Collecting failed test reports for $TAG"
  cp junit.xslt $TEST_RUN_FOLDER/$TAG/build/test-results/test/.
  docker run --rm -v "$TEST_RUN_FOLDER/$TAG/build/test-results/test:/wrk" $XSLTPROC_DOCKER_IMAGE \
    bash -c "ls *.xml | xargs -n 1 -I {} xsltproc -o {} junit.xslt {}"

  if [ -f $TEST_RUN_FOLDER/$TAG/build/failures.txt ]; then
    cat $TEST_RUN_FOLDER/$TAG/build/failures.txt >> $workingDir/../build/failures.txt
  fi
done

info "All failed tests for rerun"
cat $workingDir/../build/failures.txt

test_run_result=0
if [ $(wc -l $workingDir/../build/failures.txt | awk '{print $1}') -ge 1 ]
then
  test_run_result=1

  if [ $(wc -l $workingDir/../build/failures.txt | awk '{print $1}') -le 30 ]
  then
    TAGS+=(RERUN)

    cp -r $workingDir/../ $TEST_RUN_FOLDER/RERUN

    set +e

    docker run \
      --name RERUN \
      --rm \
      --network $NETWORK \
      --env-file vars_test_runner.env \
      --env-file vars_ci_run.env \
      -v $TEST_RUN_FOLDER/RERUN/:/repo \
      $DOCKER_IMAGE bash -c " \
        cd /repo ; \
        $BROWSERSTACK_LOCAL_STRING \
        BROWSERSTACK_ACCESSKEY=$BROWSERSTACK_ACCESSKEY BROWSERSTACK_USERNAME=$BROWSERSTACK_USERNAME $URLSUFFIX \
        APP_PATH=$BROWSERSTACK_APPPATH BROWSERSTACK_LOCAL_IDENTIFIER=$NETWORK $DEVICENAME $OSVERSION $APPSCHEME $AUTOLOGIN \
        ./gradlew rerun --no-daemon --stacktrace \
          -Dcucumber.options=\"--strict \" \
          -Dwebdriver.provided.type=$BROWSER \
          $APPIUM_TYPE \
          -Dwebdriver.base.url=$(cat vars_test_runner.env | grep url | cut -f2 -d'='); \
        test_run_result=\$?; \
        chown -R $USER_ID:$GROUP_ID /repo; \
        exit \$test_run_result"

    test_run_result=$?

    set -e
  fi
fi

# Aggregate test results
info "Aggregating test results"
info "Collecting reports"
for TAG in ${TAGS[*]}; do
  info "Collecting serenity reports for $TAG"
  cp -r $TEST_RUN_FOLDER/$TAG/target/site/serenity $workingDir/../target/site/.
  cp -r $TEST_RUN_FOLDER/$TAG/target/site/Gherkin-Report.html $workingDir/../target/site/
  if [ -d $TEST_RUN_FOLDER/$TAG/build/test-results ]
  then
    mkdir -p $workingDir/../build/test-results/$TAG
    cp -r $TEST_RUN_FOLDER/$TAG/build/test-results/* $workingDir/../build/test-results/$TAG/.
  else
    mkdir -p $workingDir/../build/test-results/test
    cp $workingDir/../TEST-TrancheFailed.xml $workingDir/../build/test-results/test/TEST-$TAG.xml
    sed -i '' -e s/name=\"\"/name=\"$TAG\"/ $workingDir/../build/test-results/test/TEST-$TAG.xml
    sed -i '' -e s/classname=\"\"/classname=\"$TAG\"/ $workingDir/../build/test-results/test/TEST-$TAG.xml
  fi
  cp -r $TEST_RUN_FOLDER/$TAG/$ACCESSIBILITY_OUTPUT $workingDir/../.
done

if [ "$MODE" == "teamcity" ]
then
  docker run \
    --cpus $TC_CPUS \
    --memory $TC_RAM \
    --rm \
    -v $workingDir/../:/repo \
    $DOCKER_IMAGE bash -c " \
      set -e ; \
      cd /repo ; \
      ./gradlew --no-daemon aggregate; \
      chown -R $USER_ID:$GROUP_ID /repo"
else
  docker run \
    --rm \
    -v $workingDir/../:/repo \
    $DOCKER_IMAGE bash -c " \
      set -e ; \
      cd /repo ; \
      ./gradlew --no-daemon aggregate; \
      chown -R $USER_ID:$GROUP_ID /repo"
fi

mkdir -p logs

for TAG in ${TAGS[*]}; do

  for CONTAINER_ID in $(docker-compose -p ${TAG} $DOCKER_COMPOSE_FILES_ARG ps -q); do
    CONTAINER_NAME=$(docker ps -a --filter "id=$CONTAINER_ID" --format '{{.Names}}')
    info "Fetching logs for $CONTAINER_NAME ($CONTAINER_ID)"
    docker logs $CONTAINER_NAME >& ./logs/$CONTAINER_NAME.log
  done

  docker-compose -p $TAG $DOCKER_COMPOSE_FILES_ARG stop

done

if [ -z "$TF_BUILD" ]; then
  #cleanup
  rm -rf $TEST_RUN_FOLDER
  rm -rf docker-compose.ci-run.yml
  rm -rf vars_ci_run.env

  if [ $MODE == "teamcity" ]; then
    info "Deleting all docker containers"
    CONTAINERS=$(docker ps -aq)
    [ -z "$CONTAINERS" ] || docker rm -f $CONTAINERS

    exit
  fi;

  exit $test_run_result
fi;

if [ $test_run_result -ne 0 ]; then
  echo "##vso[task.logissue type=error]BDD Tests Failed"
  echo "##vso[task.complete result=Failed;]BDD Tests Failed"
fi;
