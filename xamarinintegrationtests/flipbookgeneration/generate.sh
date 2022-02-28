#! /usr/bin/env bash

node generator.js
xvfb-run wkhtmltopdf --orientation Landscape output.html flipbook/flipbook.pdf 