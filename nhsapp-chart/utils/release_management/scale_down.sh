#!/bin/bash
NAMESPACE="${ENV}"
RELEASE_VERSION="${APPVERSION}"

# Scale HPA and PDB down for prior releases
HPAS=$(kubectl get hpa -n ${NAMESPACE} --selector "version!=${RELEASE_VERSION}" --output='name')
PDBS=$(kubectl get pdb -n ${NAMESPACE} --selector "version!=${RELEASE_VERSION}" --output='name')

while IFS= read -r p; do
  kubectl patch -n "${NAMESPACE}" "${p}" --patch '{"spec": {"minAvailable":0}}'
done <<< "${PDBS}"

while IFS= read -r h; do
  kubectl patch -n "${NAMESPACE}" "${h}" --patch '{"spec": {"minReplicas":1}}'
done <<< "${HPAS}"

PODS=$(kubectl get pod -n ${NAMESPACE} --selector "version!=${RELEASE_VERSION},role!=www" --field-selector=status.phase=Running --output='name')
# Delete pods to force reboot so new CosmosDB keys can be fetched
while IFS= read -r p; do
  kubectl delete -n "${NAMESPACE}" "${p}" --wait=false
done <<< "${PODS}"