#! /usr/bin/env bash
PATH="${PATH}:node_modules/.bin"

OUTPUT_PATH="target/site"
TEXT_FILE="${OUTPUT_PATH}/Gherkin-Report.txt"
HTML_REPORT="${OUTPUT_PATH}/Gherkin-Report.html"

function generate_html() {
  local textReport=$(<"${TEXT_FILE}")

  if [ -z "${textReport}" ]; then
    textReport="-- No linting issues found --"
  fi

  local html=`cat <<EOF
    <html>
      <head>
        <title>BDD Gherkin Linter Report</title>
      </head>

      <body>
        <h1 style="color: #5e9ca0;">BDD Gherkin Linter Report</h1>
        <pre>${textReport}</pre>
      </body>
    </html>
EOF
`

  echo "${html}" > "${HTML_REPORT}"
}

function clean_output() {
  mkdir -p "${OUTPUT_PATH}"

  rm -f "${TEXT_FILE}" "${HTML_FILE}"
}

function lint() {
  clean_output

  gherkin-lint 'src/test/kotlin/features/**/*' 2>&1 \
    | strip-ansi \
    | tee "${TEXT_FILE}"

  RESULT=${PIPESTATUS[0]}

  generate_html

  exit $RESULT
}

lint
