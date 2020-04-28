import AuthorisationService from '@/services/authorisation-service';
import auth from '@/middleware/auth';
import { APPOINTMENTS,
  BEGINLOGIN,
  LOGIN,
  INDEX,
  INTERSTITIAL_REDIRECTOR,
  APPOINTMENT_BOOKING,
  REDIRECT_PARAMETER,
} from '@/lib/routes';

jest.mock('@/services/authorisation-service');

describe('middleware/auth', () => {
  let app;
  let getters;
  let redirect;
  let store;
  const generatedLoginUrl = 'test_foo';

  const environment = {
    NATIVE_CID_REDIRECT_URI: 'mock native cid redirect uri',
    CID_REDIRECT_URI: 'mock cid redirect uri',
    CID_CLIENT_ID: 'mock cid client ID',
    CID_AUTH_ENDPOINT: 'mock cid auth endpoint',
  };

  const callAuth = (route) => {
    auth({ app, store, redirect, route });
  };

  beforeEach(() => {
    AuthorisationService.mockImplementation(() => ({
      generateLoginUrl: jest.fn(() => ({ loginUrl: generatedLoginUrl })),
    }));
    getters = [];
    store = {
      $cookies: {
        set: jest.fn(),
      },
      state: {
        device: {
          isNativeApp: true,
        },
      },
      getters,
    };
    redirect = jest.fn();
    app = {};
  });

  describe('isloggedIn is true', () => {
    beforeEach(() => {
      getters['session/isLoggedIn'] = () => true;
      callAuth(APPOINTMENT_BOOKING);
    });

    it('will not be redirected', () => {
      expect(redirect).not.toBeCalled();
    });

    describe('when path is LOGIN', () => {
      beforeEach(() => {
        callAuth(LOGIN);
      });

      it('will be redirected to the index page', () => {
        expect(redirect).toBeCalledWith(INDEX.path);
      });
    });
  });

  describe('isloggedIn is false', () => {
    beforeEach(() => {
      getters['session/isLoggedIn'] = () => false;
    });

    describe('when is a known route', () => {
      beforeEach(() => {
        callAuth(APPOINTMENT_BOOKING);
      });

      it('will be redirected to the login page with redirecturl query param', () => {
        expect(redirect).toBeCalledWith(`${LOGIN.path}?${REDIRECT_PARAMETER}=${APPOINTMENT_BOOKING.name}`);
      });
    });

    describe('when route is the index page', () => {
      beforeEach(() => {
        callAuth(INDEX);
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(redirect).toBeCalledWith(LOGIN.path);
      });
    });

    describe('when route is the redirector page', () => {
      beforeEach(() => {
        callAuth(INTERSTITIAL_REDIRECTOR);
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(redirect).toBeCalledWith(LOGIN.path);
      });
    });

    describe('when route is the redirector page with a redirect parameter', () => {
      beforeEach(() => {
        callAuth(
          { ...INTERSTITIAL_REDIRECTOR,
            query: { [REDIRECT_PARAMETER]: APPOINTMENTS.name } },
        );
      });

      it('will be redirected to the login page with the same redirect parameter', () => {
        expect(redirect).toBeCalledWith(`${LOGIN.path}?${REDIRECT_PARAMETER}=${APPOINTMENTS.name}`);
      });
    });

    describe('is the BEGINLOGIN route', () => {
      beforeEach(() => {
        app.$env = environment;
        getters['session/isLoggedIn'] = () => false;
        callAuth({ ...BEGINLOGIN,
          query: { source: 'ios' } });
      });

      it('will be redirected to the generated cid url', () => {
        expect(redirect).toBeCalledWith(generatedLoginUrl);
      });
    });
  });
});
