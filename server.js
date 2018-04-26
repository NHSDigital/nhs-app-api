/* eslint no-console: off */
const minimist = require('minimist');
const express = require('express');
const path = require('path');
const config = require('./config/env');

const app = express();
const args = minimist(process.argv.slice(2));

app.use(express.static(path.resolve(`${__dirname}/dist`)));

// Environment configuration endpoint
app.get('/config', (request, response) => {
  response.send(config);
});

app.get('/.well-known/assetlinks.json', (request, response) => {
  response.type('application/json');
  response.sendFile(path.resolve(`${__dirname}/app_links`, 'assetlinks.json'));
});

app.get('/apple-app-site-association', (request, response) => {
  response.type('application/json');
  response.sendFile(path.resolve(`${__dirname}/app_links`, 'apple-app-site-association'));
});

app.get('*', (request, response) => {
  response.sendFile(path.resolve(`${__dirname}/dist`, 'index.html'));
});

const port = args.port || config.PORT;
console.log(`Starting Service on Port ${port}`);
app.listen(port);
