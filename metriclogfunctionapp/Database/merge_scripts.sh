#! /usr/bin/env bash

set +e

# Change current working directory to be the Database directory, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")" || exit 1

OUTPUT_FILE=${OUTPUT_FILE:-update.sql}

echo "
SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';
" > "${OUTPUT_FILE}"

SCHEMAS=()
for SCHEMA_PATH in Prod/*/; do

  SCHEMA_REGEX='^Prod/(.+)/$'
  if [[ $SCHEMA_PATH =~ $SCHEMA_REGEX ]]; then
    SCHEMA="${BASH_REMATCH[1]}"
  else
    echo "Failed to parse schema name from ${SCHEMA_PATH}"
    exit 1
  fi

  echo "
CREATE SCHEMA IF NOT EXISTS ${SCHEMA};
" >> "${OUTPUT_FILE}"
  SCHEMAS+=("${SCHEMA}")
done

SCRIPT_FILES=("Prod/Users.sql" "Prod/PermissionProceduresCreate.sql")

function add_script_files() {
  local DIR=$1

  if [ -d "$DIR" ]; then
    IFS=$'\n'
    for file in $(find "${DIR}" -name '*.sql' | sort); do
      SCRIPT_FILES+=("$file")
    done;
    unset IFS
  fi
}

function add_schema_script_files() {
  local TYPE=$1
  local SCHEMA

  for SCHEMA in "${SCHEMAS[@]}"; do
    add_script_files "Prod/${SCHEMA}/${TYPE}"
  done
}

add_schema_script_files "tables"
add_schema_script_files "views"
add_schema_script_files "procedures"
add_schema_script_files "functions"
add_schema_script_files "sequences"
add_script_files "Migrations"

SCRIPT_FILES+=("Prod/PermissionProceduresDrop.sql")

cat "${SCRIPT_FILES[@]}" >> "${OUTPUT_FILE}"
