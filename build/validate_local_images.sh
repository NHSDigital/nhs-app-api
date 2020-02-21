#! /usr/bin/env bash

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source build/lib/set_env.sh

MISSING_IMAGES=0
for IMAGE in "${IMAGE_SETTING_NAMES[@]}"; do
    REGISTRY_VAR="${IMAGE}_DOCKER_REGISTRY"
    REGISTRY=${!REGISTRY_VAR:-local};

    if [ "$REGISTRY" == "local" ]; then
        TAG_VAR="${IMAGE}"_DOCKER_TAG
        TAG=${!TAG_VAR:-latest};

        if [ "$IMAGE" == "SJRCONFIG" ]; then
            IMAGE="service-journey-dev-config"
        elif [ "$IMAGE" == "SJRAPI" ]; then
            IMAGE="backendservicejourneyrulesapi"
        elif [ "$IMAGE" == "LOGAPI" ]; then
            IMAGE="clientloggerapi"
        elif [ "$IMAGE" == "WEB" ]; then
            IMAGE="web"
        else
            IMAGE=$(echo "backend${IMAGE}" | tr '[:upper:]' '[:lower:]')
        fi;

        IMAGE_REF=$REGISTRY/nhsonline-${IMAGE}:$TAG

        if [ -z "$(docker image ls -q --filter=reference="$IMAGE_REF")" ]; then
            echo Missing image "$IMAGE_REF"
            MISSING_IMAGES=$((MISSING_IMAGES+1))
        fi;
    fi;
done;

if [ $MISSING_IMAGES != 0 ]; then
    echo
    echo "Local images are missing, please run a build (make build)"
    exit 1
fi;
