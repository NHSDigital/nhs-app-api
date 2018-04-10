#!/bin/bash

git-crypt unlock ~/.ci-key.gpg
mkdir -p ~/.kube
echo $KUBECTL_CONFIG | base64 -d | zcat > ~/.kube/config
sed -i "s/latest/$(git rev-parse HEAD)/" kubernetes/deployment.yaml
kubectl apply -f kubernetes/