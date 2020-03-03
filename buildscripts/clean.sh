#! /usr/bin/env bash

CONTAINERS=$(docker ps -a | awk '{print $1}' | grep -v CONTAINER)
[ -z "$CONTAINERS" ] || docker rm -f $CONTAINERS

VOLUMES=$(docker volume list | awk '{print $2}' | grep -v VOLUME)
[ -z "$VOLUMES" ] || docker volume rm -f $VOLUMES
