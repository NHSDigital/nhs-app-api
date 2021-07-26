#!/bin/bash

BASE_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )/" && pwd )"
GP_AT_HAND_GP_INFO="$BASE_DIR/gpathandpractices.csv"
OUT_GP_AT_HAND_FOLDER="$BASE_DIR/../../configurations/Journeys/gpAtHand"

while IFS=, read -r ods
do

	cat >${OUT_GP_AT_HAND_FOLDER}/odscode_${ods}.yaml <<EOT
\$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCode: "${ods}"
journeys:
  appointments:
    provider: gpAtHand
EOT

done < $GP_AT_HAND_GP_INFO

