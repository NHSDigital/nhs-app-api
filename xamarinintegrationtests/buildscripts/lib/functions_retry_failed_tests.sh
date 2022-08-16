#! /usr/bin/env bash

function ParseFailedTestMethodNames()
{
  local testRunOutputFileName="$1"
	RETRIED_TESTS_METHOD_NAMES=$(jq -jr '([.[] | select(.outcome=="Failed")] | unique_by(.methodName)) | .[] | .methodName, "|"' < "$testRunOutputFileName")
	# Trim trailing '|' char
	RETRIED_TESTS=${RETRIED_TESTS_METHOD_NAMES%?}

	printf "Failed tests to retry:\n"
	IFS='|' read -ra RETRIED_TEST_NAME <<< "$RETRIED_TESTS"
	for testName in "${RETRIED_TEST_NAME[@]}"; do
		# Test may have passed in another run
		hasTestPassedInOtherRun=$(jq -jr '.[] | select(.outcome=="Passed" and .methodName=="'$testName'") | length' "$testRunOutputFileName")
		if [ -z "$hasTestPassedInOtherRun" ]
	  then
	    printf "%s\n" "$testName"
	  	RETRIED_TEST_FILTER_STRING+="Name=$testName|"
	  fi

	done

	# Trim trailing '|' char
	RETRIED_TEST_FILTER_STRING=${RETRIED_TEST_FILTER_STRING%?}
}