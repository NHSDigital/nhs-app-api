import AuthorisationService from '@/services/authorisation-service';
import auth from '@/middleware/auth';
import {
  INDEX_NAME,
  LOGIN_NAME,
  APPOINTMENTS_NAME,
  REDIRECT_PARAMETER,
} from '@/router/names';
import { EMPTY_PATH } from '@/router/paths';
import { BEGIN_LOGIN, LOGIN } from '@/router/routes/login';
import { BOOKING } from '@/router/routes/appointments';
import { REDIRECTOR } from '@/router/routes/general';
import * as dependancy from '@/lib/utils';

jest.mock('@/services/authorisation-service');
jest.mock('@/lib/utils');

describe('middleware/auth', () => {
  let getters;
  let store;
  const generatedLoginUrl = 'test_foo';
  const $router = {
    push: jest.fn(),
    getMatchedComponents: jest.fn(false),
  };
  const next = jest.fn();

  const environment = {
    NATIVE_CID_REDIRECT_URI: 'mock native cid redirect uri',
    CID_REDIRECT_URI: 'mock cid redirect uri',
    CID_CLIENT_ID: 'mock cid client ID',
    CID_AUTH_ENDPOINT_URL: 'mock cid auth endpoint',
  };

  const callAuth = (route) => {
    auth({ store, to: route, next, router: $router });
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
      app: {
        $router,
        isNhsAppPath: jest.fn(),
      },
    };
    dependancy.createRouteByNameObject = jest.fn();
    dependancy.createRoutePathObject = jest.fn();
    dependancy.checkIfPathShouldHavePatientPrefix.mockImplementation(x => x.path);
  });

  describe('isloggedIn is true', () => {
    const query = 'query';
    const params = 'param';
    beforeEach(() => {
      getters['session/isLoggedIn'] = () => true;
      const to = {
        ...BOOKING,
        matched: [
          { ...BOOKING },
        ],
      };
      callAuth(to);
    });

    it('will not be redirected', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });

    describe('when path is LOGIN', () => {
      beforeEach(() => {
        const to = {
          ...LOGIN,
          query,
          params,
          store,
        };
        callAuth(to);
      });

      it('will be redirected to the index page', () => {
        expect(dependancy.createRouteByNameObject).toBeCalledWith({
          name: INDEX_NAME,
          query,
          params,
          store,
        });
      });
    });

    describe('when route is not matched', () => {
      const to = {
        path: '/booking',
        matched: [],
        query,
        params,
        store,
      };
      it('will not redirect if path is invalid', () => {
        dependancy.checkIfPathShouldHavePatientPrefix.mockImplementation(undefined);
        callAuth(to);
        expect(dependancy.createRoutePathObject).not.toBeCalledWith(expect.anything);
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });

      it('will redirect if path is valid', () => {
        callAuth(to);
        expect(dependancy.createRoutePathObject).toBeCalledWith({
          path: to.path,
          query,
          params,
          store,
        });
      });
    });
  });

  describe('isloggedIn is false', () => {
    beforeEach(() => {
      getters['session/isLoggedIn'] = () => false;
    });

    describe('when is a known route', () => {
      beforeEach(() => {
        callAuth(BOOKING);
      });

      it('will be redirected to the login page with redirecturl query param', () => {
        const query = { [REDIRECT_PARAMETER]: BOOKING.name };
        expect(next).toBeCalledWith({ name: LOGIN_NAME, query });
      });
    });

    describe('when route is an empty path', () => {
      beforeEach(() => {
        callAuth({ path: EMPTY_PATH });
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME });
      });
    });

    describe('when route has no name', () => {
      beforeEach(() => {
        callAuth({ path: 'some_path' });
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME });
      });
    });

    describe('when route is the redirector page', () => {
      beforeEach(() => {
        callAuth(REDIRECTOR);
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME });
      });
    });

    describe('when route is the redirector page with a redirect parameter', () => {
      const query = {};
      query[REDIRECT_PARAMETER] = APPOINTMENTS_NAME;
      beforeEach(() => {
        callAuth({
          ...REDIRECTOR,
          query,
        });
      });

      it('will be redirected to the login page with the same redirect parameter', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME, query });
      });
    });

    describe('is the BEGINLOGIN route', () => {
      const { location } = window;
      const hostname = 'www.example.com';

      beforeEach(() => {
        store.$env = environment;
        getters['session/isLoggedIn'] = () => false;

        delete window.location;
        window.location = {
          hostname,
        };

        callAuth({ ...BEGIN_LOGIN,
          query: { source: 'ios' } });
      });

      it('will be redirected to the generated cid url', () => {
        expect(window.location.href).toEqual(generatedLoginUrl);
        expect(next).toBeCalledWith(false);
      });

      afterEach(() => {
        window.location = location;
      });
    });
  });
});
