#!/bin/bash

if [[ -z  $KUBECTL_CONFIG  ]];
then
	echo "error: KUBECTL_CONFIG variable is not set!"
	exit 1
fi

git-crypt unlock ~/.ci-key.gpg
mkdir -p ~/.kube
echo $KUBECTL_CONFIG | base64 -d | zcat > ~/.kube/config
sed -i "s/latest/$(git rev-parse HEAD)/" kubernetes/deployment.yaml
kubectl apply -f kubernetes/