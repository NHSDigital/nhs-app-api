import bodyParser from 'body-parser';
import express, { Router } from 'express';

import {
  ORGAN_DONATION,
  ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_YOUR_CHOICE,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
} from '@/lib/routes';
import appointments from '@/middleware-server/routes/appointments';
import brothermailer from '@/middleware-server/routes/brothermailer';
import organDonation from '@/middleware-server/routes/organ-donation';


const postEndpoints = [
  ORGAN_DONATION,
  ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_YOUR_CHOICE,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
].map(x => x.path);

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
