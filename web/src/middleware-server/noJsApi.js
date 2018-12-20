import bodyParser from 'body-parser';
import express, { Router } from 'express';
import appointments from '@/middleware-server/routes/appointments';
import brothermailer from '@/middleware-server/routes/brothermailer';
import organDonation from '@/middleware-server/routes/organ-donation';


const postEndpoints = [
  '/organ-donation/confirm',
];

const app = express();

app.use(bodyParser.urlencoded({ extended: true }));
app.use(appointments());
app.use(brothermailer());
app.use(organDonation());

const router = Router();
postEndpoints.map(path => router.post(path, (req, res, next) => next()));
app.use(router);

module.exports = {
  path: '/',
  handler: app,
};
