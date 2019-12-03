import appointments from '@/middleware-server/routes/appointments';
import { APPOINTMENTS } from '@/lib/routes';
import { createUri } from '@/lib/noJs';

const patientId = 1;
let api;
let apiFn;
let post;
let postHandler;
let req;
let res;
let router;

describe('middleware-server/routes/appointments', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    api = {
      getV1PatientConfiguration: jest.fn(() => Promise.resolve({ id: patientId })),
      deleteV1PatientAppointments: jest.fn(() => Promise.resolve()),
    };
    apiFn = jest.fn(() => api);
    req = {};
    res = {
      redirect: jest.fn(),
    };

    router = appointments(apiFn);

    // eslint-disable-next-line prefer-destructuring
    post = router.post;
  });

  describe('POST /nojs/appointments/cancel', () => {
    beforeEach(() => {
      // eslint-disable-next-line prefer-destructuring
      postHandler = post.mock.calls[0][1];
    });

    it('will create the NHSOnlineApi client using the received function', async () => {
      await postHandler(req, res);
      expect(apiFn).toBeCalled();
    });

    it('will pass the expected options to the NHSOnlineApi client', async () => {
      await postHandler(req, res);
      const options = apiFn.mock.calls[0][0];
      expect(options).toHaveProperty('req', req);
      expect(options).toHaveProperty('res', res);
      expect(options).toHaveProperty('store.dispatch');
      expect(options).toHaveProperty('store.app.$env', process.env);
    });

    it('will be initialized to respond to request', () => {
      expect(post).toBeCalled();
      expect(post.mock.calls[0][0]).toEqual('/nojs/appointments/cancel');
    });

    it('will call the "deleteV1PatientAppointments" endpoint with an appointmentCancelRequest', async () => {
      expect(post).toBeCalled();

      await postHandler(req, res);

      expect(api.deleteV1PatientAppointments).toBeCalled();
      expect(api.deleteV1PatientAppointments.mock.calls[0][0]).toHaveProperty('appointmentCancelRequest');
    });

    it('will call "deleteV1PatientAppointments" passing properties from the request body', async () => {
      req.body = {
        id: 21,
        reason: 'nooo!',
      };

      await postHandler(req, res);

      const requestArgument = api.deleteV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.appointmentCancelRequest.appointmentId).toEqual(req.body.id);
      expect(requestArgument.appointmentCancelRequest.cancellationReasonId)
        .toEqual(req.body.reason);
    });

    it('will pass the cookies from the API request through to NHSOnlineApi', async () => {
      req.headers = {
        cookie: '123abc',
      };

      await postHandler(req, res);
      const patientRequestArgument = api.getV1PatientConfiguration.mock.calls[0][0];
      expect(patientRequestArgument.cookie).toEqual(req.headers.cookie);

      const requestArgument = api.deleteV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.cookie).toEqual(req.headers.cookie);
      expect(requestArgument.patientId).toEqual(patientId);
    });

    it('will pass the csrfToken from the API request through to the NHSOnlineApi', async () => {
      req.body = {
        csrfToken: '123abc',
      };

      await postHandler(req, res);
      const requestArgument = api.deleteV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.csrfToken).toEqual(req.body.csrfToken);
    });

    it('will redirect back to appointments on successful response from API', async () => {
      const successMessage = {
        flashMessage: {
          show: true,
          key: 'appointments.cancelling.successText',
        },
      };
      const uri = createUri({ path: APPOINTMENTS.path, noJs: successMessage });
      await postHandler(req, res);
      expect(res.redirect).toBeCalledWith(uri);
    });
  });
});
