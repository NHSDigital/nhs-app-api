import get from 'lodash/fp/get';
import { Router } from 'express';
import { APPOINTMENTS } from '@/lib/routes';
import NHSOnlineApi from '@/services/nhsonlineapi';

const createOptions = ({ req, res, env }) => ({
  req,
  res,
  store: {
    dispatch: () => {},
    app: {
      $env: env,
    },
  },
});

export default (apiFn) => {
  const router = Router();

  router.post('/appointments/book', async (req, res) => {
    const options = createOptions({ req, res, env: process.env });
    const api = apiFn ? apiFn(options) : new NHSOnlineApi(options);
    const { bookingReason, csrfToken, endTime, slotId, startTime } = get('body')(req) || {};

    const appointmentBookRequest = {
      BookingReason: bookingReason,
      EndTime: new Date(endTime),
      SlotId: slotId,
      StartTime: new Date(startTime),
    };

    await api.postV1PatientAppointments({
      appointmentBookRequest,
      cookie: get('headers.cookie')(req),
      csrfToken,
    });

    res.redirect(APPOINTMENTS.path);
  });

  return router;
};
