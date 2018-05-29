#!/bin/bash

## Dependencies: X11 server - XQuartz for MacOS, XMing for Windows, Docker

#### 1. First login to azure docker registry (you can do it by running docker-login.sh script from keybase repo)
#### 2. Then check if your repo names match default ones (if not change them in docker-compose_ci.yml from i.e. `context: ./../nhsonline-web/` to `context: ./../your_name_of_web_repo/`)
# set -x
DOCKER_IMAGE_CHROME=nhsonline.azurecr.io/chrome:latest
DOCKER_IMAGE_FIREFOX=nhsonline.azurecr.io/firefox:latest

IP_local_docker=$(ifconfig en0 | grep inet | awk '$1=="inet" {print $2}')
xhost + $IP_local_docker

#### 3. Change browser variable to one webdriver mentioned in ./serenity.properties
BROWSER=chromenongpu

#### 4. Change an image to appropriate one (with proper browser inside, it needs to match your previous choice :D)
DOCKER_IMAGE=$DOCKER_IMAGE_CHROME

docker-compose -f docker-compose_ci.yml up -d --build

##################### Runtime vars
WEB_ID=$(docker ps -qf ancestor=nhsonline.azurecr.io/nhsonline-web:latest)
NETWORK=$(docker inspect $WEB_ID --format '{{range .NetworkSettings.Networks}}{{.NetworkID}}{{end}}' | cut -c 1-12)
#####################

docker run \
  -it \
  --rm \
  --network $NETWORK \
  --env-file vars_ci.env \
  -v $(pwd)/../:/repo \
  -e DISPLAY=$IP_local_docker:0 \
  -v /tmp/.X11-unix:/tmp/.X11-unix \
  $DOCKER_IMAGE /bin/bash

test_exit_code=$?

docker-compose -f docker-compose_ci.yml stop
docker rm $(docker ps -aq)

exit "$test_exit_code"

# cd /repo ; ./gradlew clean test aggregate -Dcucumber.options='--tags ~@bug --tags ~@pending' -Dwebdriver.provided.type=chromenongpu -Dwebdriver.base.url=http://nhsonline.web:3000
