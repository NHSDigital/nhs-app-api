#! /usr/bin/env bash

TAGS_NEVER_RUN=(bug pending manual tech-debt)
TAGS_TRANCHE_RUN=(organ-donation prescription appointments my-record)
TAGS_CUSTOM_RUN=(native cosmos accessibility onlineconsultations long-running appointments-book)

function join_by { local d=$1; shift; echo -n "$1"; shift; printf "%s" "${@/#/$d}"; }

function generate_tags () {
  local TAG_TO_RUN EXCLUDE_TAGS EXCLUDE_TAGS_ARGS
  TAG_TO_RUN=$1

  EXCLUDE_TAGS=("${TAGS_NEVER_RUN[@]}")
  for TAG_CUSTOM_RUN in "${TAGS_CUSTOM_RUN[@]}"; do
    if [ -n "$TAG_CUSTOM_RUN" ] && [ "$TAG_CUSTOM_RUN" != "$TAG_TO_RUN" ] && [ "$TAG_TO_RUN" != "smoketest" ]; then
      EXCLUDE_TAGS+=("$TAG_CUSTOM_RUN")
    fi
  done

  EXCLUDE_TAGS_ARGS=$(join_by ' and not @' "${EXCLUDE_TAGS[@]}")
  TAGS="@$TAG_TO_RUN and not @$EXCLUDE_TAGS_ARGS"

  info "TAGS: $TAGS"
}

function generate_tags_others () {
  local EXCLUDE_TAGS_ARGS

  EXCLUDE_TAGS_ARGS=$(join_by ' and not @' "${TAGS_NEVER_RUN[@]}" "${TAGS_CUSTOM_RUN[@]}" "${TAGS_TRANCHE_RUN[@]}")
  TAGS="not @$EXCLUDE_TAGS_ARGS"

  info "TAGS: $TAGS"
}
