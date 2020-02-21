#! /usr/bin/env bash

ports=$(docker ps -q | xargs -I{} docker port {} | cut -f2 -d':' | tr '\n' ' ')

echo Setting up port forwarding for the following ports:
echo $ports | tr ' ' '\n'

ssh_options=$(echo $ports | tr ' ' '\n' | sed -n 's/.*/-L \0:0.0.0.0:\0/p')

docker-machine ssh default -N $ssh_options
