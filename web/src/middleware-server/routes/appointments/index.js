import get from 'lodash/fp/get';
import { Router } from 'express';
import { APPOINTMENTS } from '../../../lib/routes';
import NHSOnlineApi from '../../../services/v1nhsonlineapi';
import { createUri } from '../../../lib/noJs';

const initialiseApi = ({ apiFn, req, res }) => {
  const options = {
    req,
    res,
    store: {
      dispatch: () => {},
      app: {
        $env: process.env,
      },
    },
  };

  return apiFn ? apiFn(options) : new NHSOnlineApi(options);
};

export default (apiFn) => {
  const router = Router();

  router.post('/nojs/appointments/cancel', async (req, res) => {
    const api = initialiseApi({ apiFn, req, res });
    const { reason, id, csrfToken } = get('body')(req) || {};

    const appointmentCancelRequest = {
      appointmentId: id,
      cancellationReasonId: reason,
    };

    return api.deleteV1PatientAppointments({
      appointmentCancelRequest,
      cookie: get('headers.cookie')(req),
      csrfToken,
    }).then(() => {
      const successMessage = {
        flashMessage: {
          show: true,
          key: 'appointments.cancelling.successText',
        },
      };
      const uri = createUri({ path: APPOINTMENTS.path, noJs: successMessage });
      res.redirect(uri);
    });
  });

  return router;
};
