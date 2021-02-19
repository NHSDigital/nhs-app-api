#! /usr/bin/env bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

DOCKER_ARGS=()
DOCKER_ARGS+=(--rm)
DOCKER_ARGS+=(-v "${REPO_ROOT}/xamarinintegrationtests:${DOCKER_ROOT}work")

pull_docker_image "${DOCKER_IMAGE_XSLTPROC}"

while IFS= read -r -d '' file
do
  REPORT_JSON=TestResults/$(basename "${file}" .trx).json
  REPORT_MD=TestResults/$(basename "${file}" .trx).md
  REPORT_HTML=TestResults/$(basename "${file}" .trx).html
  echo "Transforming ${file} => ${REPORT_JSON}"
  docker run \
    "${DOCKER_ARGS[@]}" \
    "${DOCKER_IMAGE_XSLTPROC}" trx.xslt "${file}" > "${REPORT_JSON}"

  if [ -n "$TF_BUILD" ]; then
    echo "##vso[artifact.upload containerfolder=Xamarin Test Report;artifactname=Xamarin Test Report]$(realpath "${REPORT_JSON}")"
  fi

  RETRIED_TESTS=$(jq -r '.[] | select(.shouldRetry == true) | "* *" + .displayName + "*: " + .testFailureMessage + "\n"' < "${REPORT_JSON}")

  if [ -n "${RETRIED_TESTS}" ]; then
    printf "# Retried Tests\n%s" "${RETRIED_TESTS}" > "${REPORT_MD}"

    if [ -n "$TF_BUILD" ]; then
      echo "##vso[task.addattachment type=Distributedtask.Core.Summary;name=Test Report;]$(realpath "${REPORT_MD}")"

      for CATEGORY in $(jq -r '.[] | select(.shouldRetry == true) | .retryCategory' < "${REPORT_JSON}"); do
        echo "##vso[build.addbuildtag]${CATEGORY}"
      done
    else
      cat "${REPORT_MD}"
    fi
  fi


  echo "Creating patch to inject ${REPORT_JSON} into ${REPORT_HTML}"
  cat > "${REPORT_HTML}.patch" <<EOF
--- test-result-template.html	2021-02-22 16:58:58.000000000 +0000
+++ test-results.html	        2021-02-22 17:04:16.000000000 +0000
@@ -10,4 +10,3 @@
 <script id="TestResultJson">
-    // Test Results JSON gets injected in this script tag, uncomment the below and supply some JSON for local testing
-    // const TestResults = [];
+    const TestResults = $(<"${REPORT_JSON}");
 </script>
EOF

  echo "Patching ${REPORT_HTML}"
  patch test-results-template.html "${REPORT_HTML}.patch" --output="${REPORT_HTML}"

  if [ -n "$TF_BUILD" ]; then
    echo "##vso[task.setvariable variable=test_report_html_path]${REPORT_HTML}"
  fi

done < <(find TestResults -name '*.trx' -print0)
