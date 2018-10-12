const express = require('express');
const appointments = require('../../routes/appointments');

const app = express();
app.use(appointments);

module.exports = {
  path: '/nojs',
  handler: app,
};
