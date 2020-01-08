#!/bin/bash

TRANCHES=(SMOKE_TESTS LONG_RUNNING APPOINTMENTS APPOINTMENTS_BOOK ORGAN_DONATION PRESCRIPTION MY_RECORD ACCESSIBILITY COSMOS ONLINE_CONSULTATIONS OTHERS THROTTLING)

TRANCHE_SMOKE_TESTS='"Smoke_Tests": {"tests.name": "Smoke Tests", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "smoketest"}'
TRANCHE_LONG_RUNNING='"Long_Running": {"tests.name": "Long Running", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "long-running"}'
TRANCHE_APPOINTMENTS='"Appointments": {"tests.name": "Appointments", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "appointments"}'
TRANCHE_APPOINTMENTS_BOOK='"Appointments_Book": {"tests.name": "Appointments Book", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "appointments-book"}'
TRANCHE_ORGAN_DONATION='"Organ_Donation": {"tests.name": "Organ Donation", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "organ-donation"}'
TRANCHE_PRESCRIPTION='"Prescription": {"tests.name": "Prescription", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "prescription"}'
TRANCHE_MY_RECORD='"My_Record": {"tests.name": "My Record", "tests.script": "03_integration_test_run_tranche.sh", "tranche.tag": "my-record"}'
TRANCHE_ACCESSIBILITY='"Accessibility": {"tests.name": "Accessibility", "tests.script": "03_integration_test_run_accessibility.sh"}'
TRANCHE_COSMOS='"Cosmos": {"tests.name": "Cosmos", "tests.script": "03_integration_test_run_cosmos.sh"}'
TRANCHE_ONLINE_CONSULTATIONS='"Online_Consultations": {"tests.name": "Online Consultations", "tests.script": "03_integration_test_run_onlineconsultations.sh"}'
TRANCHE_OTHERS='"Others": {"tests.name": "Others", "tests.script": "03_integration_test_run_others.sh"}'
TRANCHE_THROTTLING='"Throttling": {"tests.name": "Throttling", "tests.script": "03_integration_test_run_throttling.sh"}'

if [ -n "$SYSTEM_PULLREQUEST_PULLREQUESTID" ]; then
  INT_TESTS_RUN_SMOKE_TESTS=${INT_TESTS_RUN_SMOKE_TESTS:-True}
else
  INT_TESTS_RUN_LONG_RUNNING=${INT_TESTS_RUN_LONG_RUNNING:-True}
  INT_TESTS_RUN_APPOINTMENTS=${INT_TESTS_RUN_APPOINTMENTS:-True}
  INT_TESTS_RUN_APPOINTMENTS_BOOK=${INT_TESTS_RUN_APPOINTMENTS_BOOK:-True}
  INT_TESTS_RUN_ORGAN_DONATION=${INT_TESTS_RUN_ORGAN_DONATION:-True}
  INT_TESTS_RUN_PRESCRIPTION=${INT_TESTS_RUN_PRESCRIPTION:-True}
  INT_TESTS_RUN_MY_RECORD=${INT_TESTS_RUN_MY_RECORD:-True}
  INT_TESTS_RUN_ACCESSIBILITY=${INT_TESTS_RUN_ACCESSIBILITY:-True}
  INT_TESTS_RUN_ONLINE_CONSULTATIONS=${INT_TESTS_RUN_ONLINE_CONSULTATIONS:-True}
  INT_TESTS_RUN_OTHERS=${INT_TESTS_RUN_OTHERS:-True}
  INT_TESTS_RUN_THROTTLING=${INT_TESTS_RUN_THROTTLING:-True}
fi

if [ "$BRANCH_TAG" == "develop" ]; then
  INT_TESTS_RUN_COSMOS=${INT_TESTS_RUN_COSMOS:-True}
fi

TRANCHES_MATRIX=""
for TRANCHE in "${TRANCHES[@]}"; do
  TRANCHE_CONFIG_VAR="INT_TESTS_RUN_$TRANCHE"
  TRANCHE_CONFIG=${!TRANCHE_CONFIG_VAR}

  echo "$TRANCHE: $TRANCHE_CONFIG"
  if [ "$TRANCHE_CONFIG" == "True" ]; then
    TRANCHE_JSON_VAR="TRANCHE_$TRANCHE"
    TRANCHE_JSON=${!TRANCHE_JSON_VAR}

    if [ -z "$TRANCHES_MATRIX" ]; then
      TRANCHES_MATRIX="{$TRANCHE_JSON"
    else
      TRANCHES_MATRIX="$TRANCHES_MATRIX,$TRANCHE_JSON"
    fi
  fi
done

if [ -n "$TRANCHES_MATRIX" ]; then
  TRANCHES_MATRIX="$TRANCHES_MATRIX}"

  echo "$TRANCHES_MATRIX"
  echo "##vso[task.setVariable variable=tranches;isOutput=true]$TRANCHES_MATRIX"
fi