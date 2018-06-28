#!/bin/bash

function die () {
    echo >&2 "===]> Error: $@ "
    exit 1
}

function info () {
    echo >&2 "===]> Info: $@ ";
}

# Check if DOCKER_TAG exists in envvar
[ -z $APP_DOCKER_TAG ] && die "APP_DOCKER_TAG is not specified, it should be so we can pin builds to a specific version rather than latest"

#### 1. First login to azure docker registry (you can do it by running docker-login.sh script from keybase repo)
#### 2. Then check if your repo names match default ones (if not change them in docker-compose_ci.yml from i.e. `context: ./../nhsonline-web/` to `context: ./../your_name_of_web_repo/`)
# set -x
if [ -z "${DOCKER_REGISTRY}" ];
then
  DOCKER_REGISTRY=nhsapp.azurecr.io
fi
DOCKER_IMAGE_CHROME=$DOCKER_REGISTRY/chrome:latest
DOCKER_IMAGE_FIREFOX=$DOCKER_REGISTRY/firefox:latest

#### 3. Change browser variable to one webdriver mentioned in ./serenity.properties
BROWSER=chromeheadless

#### 4. Change an image to appropriate one (with proper browser inside, it needs to match your previous choice :D)
DOCKER_IMAGE=$DOCKER_IMAGE_CHROME

# List all docker images in the docker compose setup
DOCKER_SERVICES=`docker-compose -f docker-compose_ci.yml config --services`

BDD_CUCUMBER_OPTIONS='--tags ~@bug --tags ~@pending --tags ~@manual --tags ~@native --tags ~@tech-debt'

# Pin versions of docker images
sed -i "s/WEB_TAG=latest/WEB_TAG=${APP_DOCKER_TAG}/" .env
sed -i "s/BACKEND_TAG=latest/BACKEND_TAG=${APP_DOCKER_TAG}/" .env
[ -z $REDIS_DATA_DOCKER_TAG ] || sed -i "s/REDIS_DATA_TAG=latest/REDIS_DATA_TAG=${REDIS_DATA_DOCKER_TAG}/" .env

# Pull images
for s in $DOCKER_SERVICES; do
  if [[ "$s" != "nhsonline.web" && "$s" != "nhsonline.backend.worker" ]]; then #Don't pull local images we've built as part of the pipeline
    docker-compose -f docker-compose_ci.yml pull $s
  fi
done

# Output list of images contained in config
docker-compose -f docker-compose_ci.yml config | grep image

docker-compose -f docker-compose_ci.yml up -d --build || die "Docker compose failure"

##################### Runtime vars
if [ -z $WEB_DOCKER_TAG ]; then
  WEB_ID=$(docker ps -qf ancestor=$DOCKER_REGISTRY/nhsonline-web:latest)
else
  WEB_ID=$(docker ps -qf ancestor=$DOCKER_REGISTRY/nhsonline-web:$WEB_DOCKER_TAG)
fi
NETWORK=$(docker inspect $WEB_ID --format '{{range .NetworkSettings.Networks}}{{.NetworkID}}{{end}}' | cut -c 1-12)
#####################

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
