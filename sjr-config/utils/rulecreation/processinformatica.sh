#!/bin/bash

BASE_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )/" && pwd )"
INFORMATICA_GP_INFO="$BASE_DIR/informaticapractices.csv"
OUT_INFORMATICA_FOLDER="$BASE_DIR/../../configurations/Journeys/informatica"

while IFS=, read -r ods name url
do

	cat >${OUT_INFORMATICA_FOLDER}/odscode_${ods}.yaml <<EOT
\$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCode: "${ods}"
journeys:
  appointments:
    provider: informatica
    informaticaUrl: ${url}
EOT

done < $INFORMATICA_GP_INFO

