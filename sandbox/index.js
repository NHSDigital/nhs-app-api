'use strict';

const path = require('path');
const http = require('http');
const express = require('express');
const oasTools = require('oas-tools');
const fs = require('node:fs');

const serverPort = 9000;

const app = express();

const config = {
    oasFile: path.resolve(__dirname, 'api/openapi.yaml'),
    controllers: path.resolve(__dirname, './controllers'),
    loglevel: 'info',
    docs: true   // Enable Swagger UI at /docs
};

console.log("Using OpenAPI file:", config.oasFile);
console.log("Exists?", fs.existsSync(config.oasFile));

try {
    // Initialize oas-tools (returns app wrapper)
    oasTools.initialize(app, config);

    // Start server
    http.createServer(app).listen(serverPort, () => {
        console.log(`Server listening on http://localhost:${serverPort}`);
        console.log(`Swagger UI available at http://localhost:${serverPort}/docs`);
    });

} catch (err) {
    console.error("Failed to initialize oas-tools:", err);
}

