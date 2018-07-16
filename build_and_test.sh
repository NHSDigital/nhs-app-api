ROOT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $ROOT_DIR

DOCKER_REGISTRY=nhsapp.azurecr.io
WEB_NAME=nhsonline-web
BACKEND_NAME=nhsonline-backendworker
TAG=$(git rev-parse HEAD)

cd web
docker build . -t $DOCKER_REGISTRY/$WEB_NAME:$TAG -f Dockerfile
docker tag $DOCKER_REGISTRY/$WEB_NAME:$TAG $DOCKER_REGISTRY/$WEB_NAME:latest

cd ../backendworker
docker build . -t $DOCKER_REGISTRY/$BACKEND_NAME:$TAG -f NHSOnline.Backend.Worker/Dockerfile
docker tag $DOCKER_REGISTRY/$BACKEND_NAME:$TAG $DOCKER_REGISTRY/$BACKEND_NAME:latest

cd ../bddtests/ops
APP_DOCKER_TAG=$TAG DOCKER_REGISTRY=$DOCKER_REGISTRY ./docker_tests.sh
