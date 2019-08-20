#!/bin/bash

TEXT_FILE="target/site/Gherkin-Report.txt"
HTML_REPORT="target/site/Gherkin-Report.html"

echo "" > $TEXT_FILE
echo "" > $HTML_REPORT

function generateHTMLUpper() {
  {
  echo "<html>"
  echo "<head>"
  echo "</head>"
  echo "<body>"
  echo "<h1 style=\"color: #5e9ca0;\">Gherkin Linter Report</h1>"
   } >> $HTML_REPORT
  }

function stripAndClean() {
   cat $1 | awk '{gsub(/\[0\;4m/,"");gsub(/\[24m/,"");gsub(/\[38\;5\;243m/,"");gsub(/\[0m/,"")}4' | awk '{if ($1~/\//) { print $0 "<br/>" } else { print "Line " $1; $1 = ""; print " - Error: " $0 "<br/>"}}' >> $HTML_REPORT
}

function generateHTMLower() {
  {
  echo "</body>"
  echo "</html>"
   } >> $HTML_REPORT
  }

if [ ! -f $TEXT_FILE ]; then
  echo "File '$TEXT_FILE' not found!"
  exit 1
fi

if [ ! -f $TEXT_FILE ]; then
  echo "File '$TEXT_FILE' not found!"
  exit 1
fi


node_modules/.bin/gherkin-lint 'src/test/kotlin/features/**/*' 2>&1 | tee $TEXT_FILE
RESULT=${PIPESTATUS[0]}
generateHTMLUpper
cat $TEXT_FILE | grep -e ".*0;" -e "0m$" | stripAndClean
generateHTMLower

exit $RESULT