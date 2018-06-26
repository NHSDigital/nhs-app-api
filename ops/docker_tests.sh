#!/bin/bash

# set -x
function die () {
    echo >&2 "===]> Error: $@ "
    exit 1
}

function info () {
    echo >&2 "===]> Info: $@ ";
}

#### 1. First login to azure docker registry (you can do it by running docker-login.sh script from keybase repo)
#### 2. Then check if your repo names match default ones (if not change them in docker-compose_ci.yml from i.e. `context: ./../nhsonline-web/` to `context: ./../your_name_of_web_repo/`)
if [ -z "${DOCKER_REGISTRY}" ];
then
  DOCKER_REGISTRY=nhsapp.azurecr.io
fi
DOCKER_IMAGE_CHROME=$DOCKER_REGISTRY/chrome:latest
DOCKER_IMAGE_FIREFOX=$DOCKER_REGISTRY/firefox:latest

if [ "${TPP_CERT_TC}" ];
then
  TPP_CERT=$(echo ${TPP_CERT_TC} | base64 -d)
else
  TPP_CERT=$(cat ./../../nhsonline-backendworker/NHSOnline.Backend.Worker/certs/TppNhsTest.pfx)
fi

#### 3. Change browser variable to one webdriver mentioned in ./serenity.properties
BROWSER=chromeheadless

#### 4. Change an image to appropriate one (with proper browser inside, it needs to match your previous choice :D)
DOCKER_IMAGE=$DOCKER_IMAGE_CHROME

# List all docker images in the docker compose setup
DOCKER_SERVICES=`docker-compose -f docker-compose_ci.yml config --services`

if [ -z $BDD_FLAG ]; then
  # Run full BDD tests if BDD_FLAGS is undefined.
  BDD_CUCUMBER_OPTIONS='--tags ~@bug --tags ~@pending --tags ~@manual --tags ~@native --tags ~@tech-debt'
else
  case $BDD_FLAG in
    full)
      BDD_CUCUMBER_OPTIONS='--tags ~@bug --tags ~@pending --tags ~@manual --tags ~@native --tags ~@tech-debt'
    ;;
    smoketests)
      BDD_CUCUMBER_OPTIONS='--tags @smoketest --tags ~@bug --tags ~@pending --tags ~@manual --tags ~@native --tags ~@tech-debt'
    ;;
    *)
      die "Unknown BDD_FLAG value"
    ;;
  esac
fi
info "BDD_FLAG: $BDD_FLAG"
info "Cucumber options: $BDD_CUCUMBER_OPTIONS"

if ! [ -z $BDD_TEST_MODE ]; then
  case $BDD_TEST_MODE in
    web)
      info MODE=Web

      # Replace docker tags with overrides from TeamCity
      [ -z $WEB_DOCKER_TAG ] || sed -i "s/WEB_TAG=latest/WEB_TAG=${WEB_DOCKER_TAG}/" .env

      # Pull images
      for s in $DOCKER_SERVICES; do
        if ! [ "$s" = "nhsonline.web" ]; then #Do not pull the web image as we'll use the local copy
          docker-compose -f docker-compose_ci.yml pull $s
        fi
      done
    ;;
    backend)
      info MODE=Backend

      # Replace docker tags with overrides from TeamCity
      [ -z $BACKEND_DOCKER_TAG ]    || sed -i "s/BACKEND_TAG=latest/BACKEND_TAG=${BACKEND_DOCKER_TAG}/" .env
      [ -z $REDIS_DATA_DOCKER_TAG ] || sed -i "s/REDIS_DATA_TAG=latest/REDIS_DATA_TAG=${REDIS_DATA_DOCKER_TAG}/" .env

      # Pull images
      for s in $DOCKER_SERVICES; do
        if ! [ "$s" = "nhsonline.backend.worker" ]; then #Do not pull the backend image as we'll use the local copy
          docker-compose -f docker-compose_ci.yml pull $s
        fi
      done
    ;;
  esac
else
  info MODE=Default
  docker-compose -f docker-compose_ci.yml pull
fi

# Output list of images contained in config
docker-compose -f docker-compose_ci.yml config | grep image

docker-compose -f docker-compose_ci.yml up -d --build || die "Docker compose failure"

##################### Runtime vars
if [ -z $BACKEND_DOCKER_TAG ]; then
  BACKEND_ID=$(docker ps -qf ancestor=$DOCKER_REGISTRY/nhsonline-backendworker:latest)
else
  BACKEND_ID=$(docker ps -qf ancestor=$DOCKER_REGISTRY/nhsonline-backendworker:$WEB_DOCKER_TAG)
fi
NETWORK=$(docker inspect $BACKEND_ID --format '{{range .NetworkSettings.Networks}}{{.NetworkID}}{{end}}' | cut -c 1-12)
#####################

### ADD TPP cert to backend container
docker exec $BACKEND_ID /bin/bash -c " \
  mkdir -p ./certs ; \
  echo \"$TPP_CERT\" > ./certs/TppNhsTest.pfx ; \
  "

docker run \
  --rm \
  --network $NETWORK \
  --env-file vars_ci.env \
  -v $(pwd)/../:/repo \
  $DOCKER_IMAGE /bin/bash -c " \
    cd /repo ; \
    ./gradlew clean test aggregate \
      -Dcucumber.options=\"$BDD_CUCUMBER_OPTIONS\" \
      -Dwebdriver.provided.type=$BROWSER \
      -Dwebdriver.base.url=$(cat vars_ci.env | grep url | cut -f2 -d'=') \
  ;"

test_exit_code=$?

mkdir -p logs
for container in $(docker-compose -f docker-compose_ci.yml ps | tail -n +3 | awk '{print $1}' ); do
  docker logs $container >& ./logs/$container.log
done

docker-compose -f docker-compose_ci.yml stop
docker rm $(docker ps -aq)

exit "$test_exit_code"
