#!/bin/bash
NAMESPACE="${ENV}"
RELEASE_VERSION="${APPVERSION}"

function die () {
  echo >&2 "===]> Error: $* "
  exit 1
} 

[ -z "$NAMESPACE" ] && die "Namespace not specified, exiting..."
[ -z "$RELEASE_VERSION" ] && die "Release version not specified, exiting..."

# Get list of resources to be deleted and print to screen
# Extra grep at the end is required, HPA for iproxy is missing the version label--see NHSO-15146
RESOURCES=$(kubectl get all -n "${NAMESPACE}" --selector="version!=${RELEASE_VERSION},role!=www,app!=nhsapp-www" --output='name' | grep -vE '(cold|default)' | grep -v "${RELEASE_VERSION}")

# Horizontal Pod Autoscalers and Pod Disruption Budgets
while IFS= read -r r; do
  echo "kubectl delete --namespace ${NAMESPACE} ${r} --wait=false" >> prune_releases.sh
done <<<"${RESOURCES}"

chmod +x prune_releases.sh

echo "Please review the following script before approving:"
cat prune_releases.sh
