import bodyParser from 'body-parser';
import express from 'express';
import appointments from '@/middleware-server/routes/appointments';

const app = express();

app.use(bodyParser.urlencoded());
app.use(appointments());

module.exports = {
  path: '/nojs',
  handler: app,
};
