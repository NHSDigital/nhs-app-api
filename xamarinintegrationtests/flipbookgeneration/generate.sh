#! /usr/bin/env bash

node generator.js
xvfb-run wkhtmltopdf output.html flipbook/flipbook.pdf