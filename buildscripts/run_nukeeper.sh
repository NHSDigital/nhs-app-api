#! /usr/bin/env bash

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source buildscripts/lib/set_env.sh

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

# shellcheck source=lib/functions_slack.sh
source buildscripts/lib/functions_slack.sh

function join_by { local d=$1; shift; echo -n "$1"; shift; printf "%s" "${@/#/$d}"; }

[ -n "$Nukeeper_azure_devops_token" ] || die "Nukeeper_azure_devops_token must be set"
[ -n "$BUILD_REPOSITORY_URI" ] || die "BUILD_REPOSITORY_URI must be set"
[ -n "$XAMARIN_ONLY_RUN" ] || die "XAMARIN_ONLY_RUN must be set"

CHECKOUT_DIRECTORY="${AGENT_TEMPDIRECTORY:-$TEMP}/nukeeper"
CHANGE=${CHANGE:-minor}

NUKEEPER_ARGS=(repo)
NUKEEPER_ARGS+=("$BUILD_REPOSITORY_URI")
NUKEEPER_ARGS+=(--checkout-directory "$CHECKOUT_DIRECTORY")
NUKEEPER_ARGS+=(--maxpackageupdates 1000)
NUKEEPER_ARGS+=(--verbosity detailed)
NUKEEPER_ARGS+=(--targetBranch develop)

# Newer versions of this package require the .NET 5.0 SDK
NUKEEPER_EXCLUDES=('Microsoft.CodeAnalysis.CSharp.CodeStyle')
NUKEEPER_XAMARIN_INCLUDES=''

NUKEEPER_ARGS+=(--change "$CHANGE")
case "$CHANGE" in
  minor)
    NUKEEPER_ARGS+=(--age 3d)
    NUKEEPER_ARGS+=(--branchnametemplate 'feature/nhso-10482-update-{Count}-packages-{Hash}')

    # Xamarin projects do not support 'dotnet' CLI and will be upgraded manually. Revisit post move to .NET6.
    if [ "$XAMARIN_ONLY_RUN" == false ]; then
        info "Excluding Xamarin projects from main run as Nukeeper cannot build them"
        NUKEEPER_EXCLUDES+=('Xamarin')
        NUKEEPER_EXCLUDES+=('iProov')
    else
        NUKEEPER_XAMARIN_INCLUDES='Xamarin|iProov'
    fi
    ;;

  major)
    # Allow major upgrades time to bed in.
    # This also has the nice side effect of giving a two week grace period
    # to get the consolidated PR with minor updates merged in.
    NUKEEPER_ARGS+=(--age 2w)
    NUKEEPER_ARGS+=(--branchnametemplate 'feature/nhso-10482-update-{Name}-to-{Version}-{Hash}')

    # Exclude Microsoft packages upgraded to .Net 5.0
    NUKEEPER_EXCLUDES+=('Microsoft.AspNetCore.Authentication.JwtBearer')
    NUKEEPER_EXCLUDES+=('Microsoft.AspNetCore.Mvc.NewtonsoftJson')
    NUKEEPER_EXCLUDES+=('Microsoft.Extensions.Configuration')
    NUKEEPER_EXCLUDES+=('Microsoft.Extensions.DependencyInjection')
    NUKEEPER_EXCLUDES+=('Microsoft.Extensions.Http')
    NUKEEPER_EXCLUDES+=('Microsoft.Extensions.Logging')

    # Xamarin projects do not support 'dotnet' CLI and will be upgraded manually. Revisit post move to .NET6.
    if [ "$XAMARIN_ONLY_RUN" == false ]; then
        info "Excluding Xamarin projects from main run as Nukeeper cannot build them"
        NUKEEPER_EXCLUDES+=('Xamarin')
        NUKEEPER_EXCLUDES+=('iProov')
    else
        NUKEEPER_XAMARIN_INCLUDES='Xamarin|iProov'
    fi
    ;;

  *)
    die "CHANGE must be minor or major, was $CHANGE"
    ;;
esac

if [ "$XAMARIN_ONLY_RUN" == false ]; then
  NUKEEPER_ARGS+=(--exclude "($(join_by \| "${NUKEEPER_EXCLUDES[@]}"))")
else
  NUKEEPER_ARGS+=(--include "$NUKEEPER_XAMARIN_INCLUDES")
fi

nukeeper "${NUKEEPER_ARGS[@]}" > nukeeper_run_output.txt

info "Begin Nukeeper run output"
cat nukeeper_run_output.txt
info "End Nukeeper run output"

if [ "$XAMARIN_ONLY_RUN" == true ]; then
  NUKEEPER_OUTPUT=()
  mapfile -t NUKEEPER_OUTPUT < <(grep "^Selected package update of*" nukeeper_run_output.txt)

  info "Nukeeper output matched ${#NUKEEPER_OUTPUT[@]} Xamarin project packages to upgrade in BuildId: $BUILD_BUILDID"

  slack_message_body=$(build_xamarin_slack_message_body "$CHANGE" "${NUKEEPER_OUTPUT[@]}")

  if [[ "$(send_slack_message "${slack_message_body}" "$XAMARIN_SLACK_URI")" -eq 0 ]]; then
    warn "This Nukeeper run will have failed building Xamarin projects. Return a success code as the PR for these updates will be created manually".
    exit 0;
  else
    die "Nukeeper Xamarin job failed"
    exit 1;
  fi
fi
