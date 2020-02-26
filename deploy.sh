#!/usr/bin/env bash
pushd /repo

source script_helpers/dependency_check
source script_helpers/formatting
source script_helpers/aks_access

[ -z "$MODE" ] && die "You need to set the MODE for the deployment script: Valid values are build,promote,nhsapp"

info "MODE: $MODE"
case $MODE in
  nhsapp)
    info "NHSApp Mode Invoked - Executing NHSApp configuration"
    pushd nhsapp-chart/
    get_aks_access
    ./deploy_nhsapp.sh
    ;;
  sjr)
    info "SJR Mode Invoked - Executing ServiceJourneyRules configuration"
    pushd sjr-chart/
    get_aks_access
    ./deploy_sjr.sh
    ;;
  cleanup)
    info "Cleanup Mode Invoked - Non-live deployments will be deleted"
    get_aks_access
    ./cleanup.sh
    ;;
  *)
    die "Unrecognised mode $MODE, exiting"
    ;;
esac
