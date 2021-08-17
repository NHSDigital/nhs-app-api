#!/bin/bash
NAMESPACE="${ENV}"
RELEASE_VERSION="${APPVERSION}"

function die () {
  echo >&2 "===]> Error: $* "
  exit 1
} 

dry_run=false

while getopts 'd' opt; do
    case "$opt" in
        d) dry_run=true ;;
        *) echo 'error in command line parsing' >&2
           exit 1
    esac
done

[ -z "$NAMESPACE" ] && die "Namespace not specified, exiting..."
[ -z "$RELEASE_VERSION" ] && die "Release version not specified, exiting..."

# Get list of resources to be deleted and print to screen
# Extra grep at the end is required, HPA for iproxy is missing the version label--see NHSO-15146
RESOURCES=$(kubectl get all -n "${NAMESPACE}" --selector="version!=${RELEASE_VERSION},role!=www,app!=nhsapp-www" --output='name' | grep -vE '(cold|default)' | grep -v "${RELEASE_VERSION}")

# Horizontal Pod Autoscalers and Pod Disruption Budgets
while IFS= read -r r; do
  [ -z "$r" ] && die "Resources not defined, exiting..."
  echo "kubectl delete --namespace ${NAMESPACE} ${r} --wait=false" >> prune_releases.sh
done <<<"${RESOURCES}"

chmod +x prune_releases.sh

if [[ $dry_run == true ]]; then
  echo "Please review the following script before approving:"
  cat prune_releases.sh
elif [[ $dry_run == false ]]; then
  ./prune_releases.sh
else
  die "Error setting dry_run variable, exiting..."
fi

