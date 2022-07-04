#! /usr/bin/env bash

node generator.js
xvfb-run wkhtmltopdf --enable-local-file-access --orientation Landscape toc --toc-level-indentation 2em output.html flipbook/$FLIPBOOK_NAME.pdf
echo "Flipbook generated: $FLIPBOOK_NAME"