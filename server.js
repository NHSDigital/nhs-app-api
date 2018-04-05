/* eslint no-console: off */

const express = require('express');
const path = require('path');
const config = require('./config/env');

const app = express();

app.use(express.static(path.resolve(`${__dirname}/dist`)));

// Environment configuration endpoint
app.get('/config', (request, response) => {
  response.send(config);
});

app.get('/.well-known/assetlinks.json', (request, response) => {
  response.type('application/json');
  response.sendFile(path.resolve(`${__dirname}/app_links`, 'assetlinks.json'));
});

app.get('*', (request, response) => {
  console.log('request received');
  response.sendFile(path.resolve(`${__dirname}/dist`, 'index.html'));
});


console.log(`Starting Service on Port ${config.PORT}`);
app.listen(config.PORT);
