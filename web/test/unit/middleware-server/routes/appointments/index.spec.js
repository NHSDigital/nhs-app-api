import appointments from '@/middleware-server/routes/appointments';
import { APPOINTMENTS } from '@/lib/routes';
import { createUri } from '@/lib/noJs';

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
      postV1PatientAppointments: jest.fn(() => Promise.resolve()),
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

  describe('POST /appointments/book', () => {
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
      expect(post.mock.calls[0][0]).toEqual('/appointments/book');
    });

    it('will call the "postV1PatientAppointments" endpoint with an AppointmentBookRequest', async () => {
      expect(post).toBeCalled();

      await postHandler(req, res);

      expect(api.postV1PatientAppointments).toBeCalled();
      expect(api.postV1PatientAppointments.mock.calls[0][0]).toHaveProperty('appointmentBookRequest');
    });

    it('will call "postV1PatientAppointments" passing properties from the request body', async () => {
      req.body = {
        slotId: 21,
        bookingReason: 'woo!',
        startTime: 'start time',
        endTime: 'end time',
      };

      await postHandler(req, res);

      const requestArgument = api.postV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.appointmentBookRequest.SlotId).toEqual(req.body.slotId);
      expect(requestArgument.appointmentBookRequest.BookingReason).toEqual(req.body.bookingReason);
      expect(requestArgument.appointmentBookRequest.StartTime).toEqual(req.body.startTime);
      expect(requestArgument.appointmentBookRequest.EndTime).toEqual(req.body.endTime);
    });

    it('will pass the cookies from the API request through to NHSOnlineApi', async () => {
      req.headers = {
        cookie: '123abc',
      };

      await postHandler(req, res);
      const requestArgument = api.postV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.cookie).toEqual(req.headers.cookie);
    });

    it('will pass the csrfToken from the API request through to the NHSOnlineApi', async () => {
      req.body = {
        csrfToken: '123abc',
      };

      await postHandler(req, res);
      const requestArgument = api.postV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.csrfToken).toEqual(req.body.csrfToken);
    });

    it('will redirect back to appointments on successful response from API', async () => {
      req.body = {
        successMessageKey: 'key',
      };

      const successMessage = {
        flashMessage: {
          show: true,
          key: req.body.successMessageKey,
        },
      };
      const uri = createUri({ path: APPOINTMENTS.path, noJs: successMessage });
      await postHandler(req, res);
      expect(res.redirect).toBeCalledWith(uri);
    });
  });

  describe('POST /appointments/cancel', () => {
    beforeEach(() => {
      // eslint-disable-next-line prefer-destructuring
      postHandler = post.mock.calls[1][1];
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
      expect(post.mock.calls[1][0]).toEqual('/appointments/cancel');
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
      const requestArgument = api.deleteV1PatientAppointments.mock.calls[0][0];
      expect(requestArgument.cookie).toEqual(req.headers.cookie);
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
