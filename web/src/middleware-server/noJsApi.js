import bodyParser from 'body-parser';
import express from 'express';
import appointments from '@/middleware-server/routes/appointments';
import brothermailer from '@/middleware-server/routes/brothermailer';

const app = express();

app.use(bodyParser.urlencoded({ extended: true }));
app.use(appointments());
app.use(brothermailer());

module.exports = {
  path: '/nojs',
  handler: app,
};
