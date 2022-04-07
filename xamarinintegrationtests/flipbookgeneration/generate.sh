#! /usr/bin/env bash

node generator.js
xvfb-run wkhtmltopdf --orientation Landscape output.html flipbook/$FLIPBOOK_NAME.pdf

echo "Flipbook generated: $FLIPBOOK_NAME"