#! /usr/bin/env bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

REPORT_JSON=TestResults/mergeresults.json
REPORT_MD=TestResults/testretries.md
REPORT_HTML=TestResults/testresport.html

jq --slurp 'flatten' --compact-output TestResults/Xamarin\ Test\ Report/*.json > "${REPORT_JSON}"

RETRIED_TESTS=$(jq -r '.[] | select(.shouldRetry == true) | "* *" + .displayName + "*: " + .testFailureMessage + "\n"' < "${REPORT_JSON}")

if [ -n "${RETRIED_TESTS}" ]; then
  printf "# Retried Tests\n%s" "${RETRIED_TESTS}" > "${REPORT_MD}"

  if [ -n "$TF_BUILD" ]; then
    echo "##vso[task.addattachment type=Distributedtask.Core.Summary;name=Test Report;]$(realpath "${REPORT_MD}")"

    for CATEGORY in $(jq -r '.[] | select(.shouldRetry == true) | .retryCategory' < "${REPORT_JSON}" | sort -u); do
      echo "##vso[build.addbuildtag]${CATEGORY}"
      sleep 2
    done
  else
    cat "${REPORT_MD}"
  fi
fi

echo "Creating patch to inject ${REPORT_JSON} into ${REPORT_HTML}"
cat > "${REPORT_HTML}.patch" <<EOF
--- test-result-template.html	2021-02-22 16:58:58.000000000 +0000
+++ test-results.html	        2021-02-22 17:04:16.000000000 +0000
@@ -10,5 +10,3 @@
 <script id="TestResultJson">
-    // Test Results JSON gets injected in this script tag
-    // If this tag is modified also update 06_generate_integration_test_run_report.sh
-    const RealTestResults = null;
+    const RealTestResults = $(<"${REPORT_JSON}");
 </script>
EOF

echo "Patching ${REPORT_HTML}"
patch test-results-template.html "${REPORT_HTML}.patch" --output="${REPORT_HTML}"

if [ -n "$TF_BUILD" ]; then
  echo "##vso[task.setvariable variable=test_report_html_path]${REPORT_HTML}"
fi
