#!/bin/bash

set -e

DATABASE_NAME=${DATABASE_NAME:?the database name must be provided}
UPDATE_SCRIPT=${UPDATE_SCRIPT:-update.sql}

if ! psql --username "${POSTGRES_USER}" --dbname "${DATABASE_NAME}" -c 'select 1' &>/dev/null; then
  echo "Creating database ${DATABASE_NAME}"
  psql -e -v ON_ERROR_STOP=1 --username "${POSTGRES_USER}" --dbname "postgres" -c "CREATE DATABASE ${DATABASE_NAME} WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'en_US.utf8' LC_CTYPE = 'en_US.utf8';"
fi

if [ -f "${UPDATE_SCRIPT}" ]; then
  psql -e -v ON_ERROR_STOP=1 --username "${POSTGRES_USER}" --dbname "${DATABASE_NAME}" -f "${UPDATE_SCRIPT}"
else
  echo "Missing update script: ${UPDATE_SCRIPT}"
  exit 1
fi
