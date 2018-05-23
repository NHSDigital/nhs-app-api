#!/bin/bash

if [[ -z  $KUBECTL_CONFIG  ]];
then
	echo "error: KUBECTL_CONFIG variable is not set!"
	exit 1
fi

cd /repo
git-crypt unlock ~/.cicd.key
az login --service-principal -u ${ARM_CLIENT_ID} -p ${ARM_CLIENT_SECRET} --tenant ${ARM_TENANT_ID}
mkdir -p ~/.kube
az aks get-credentials -n $AKSCLUSTERNAME -g $AKSRESOURCEGROUP
sed -i "s/latest/$(git rev-parse HEAD)/" kubernetes/deployment.yaml
kubectl apply -f kubernetes/
