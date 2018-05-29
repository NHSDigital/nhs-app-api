#!/bin/bash

#### 1. First login to azure docker registry (you can do it by running docker-login.sh script from keybase repo)
#### 2. Then check if your repo names match default ones (if not change them in docker-compose_ci.yml from i.e. `context: ./../nhsonline-web/` to `context: ./../your_name_of_web_repo/`)
# set -x
DOCKER_IMAGE_CHROME=nhsonline.azurecr.io/chrome:latest
DOCKER_IMAGE_FIREFOX=nhsonline.azurecr.io/firefox:latest

#### 3. Change browser variable to one webdriver mentioned in ./serenity.properties
BROWSER=chromeheadless

#### 4. Change an image to appropriate one (with proper browser inside, it needs to match your previous choice :D)
DOCKER_IMAGE=$DOCKER_IMAGE_CHROME

docker-compose -f docker-compose_ci.yml up -d --build

##################### Runtime vars
WEB_ID=$(docker ps -qf ancestor=nhsonline.azurecr.io/nhsonline-web:latest)
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
      -Dcucumber.options='--tags ~@bug --tags ~@pending' \
      -Dwebdriver.provided.type=$BROWSER \
      -Dwebdriver.base.url=$(cat vars_ci.env | grep url | cut -f2 -d'=') \
  ;"

test_exit_code=$?

docker-compose -f docker-compose_ci.yml stop
docker rm $(docker ps -aq)

exit "$test_exit_code"
