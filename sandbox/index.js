'use strict';

const path = require('path');
const http = require('http');

const oas3Tools = require('oas3-tools');
const serverPort = 9000;

// swaggerRouter configuration
const options = {
    controllers: path.join(__dirname, './controllers')
};

const expressAppConfig = oas3Tools.expressAppConfig(path.join(__dirname, 'api/openapi.yaml'), options);
expressAppConfig.addValidator();
const app = expressAppConfig.getApp();

// Initialize the Swagger middleware
http.createServer(app).listen(serverPort, function () {
    console.log('Your server is listening on port %d (http://localhost:%d)', serverPort, serverPort);
    console.log('Swagger-ui is available on http://localhost:%d/docs', serverPort);
});

