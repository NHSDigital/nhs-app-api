import AuthorisationService from '@/services/authorisation-service';
import auth from '@/middleware/auth';
import {
  INDEX_NAME,
  LOGIN_NAME,
  APPOINTMENTS_NAME,
  REDIRECT_PARAMETER,
  INTEGRATION_REFERRER_PARAMETER,
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
  let generateLoginUrlMock;
  const generatedLoginUrl = 'test_foo';
  const $router = {
    push: jest.fn(),
    getMatchedComponents: jest.fn(false),
  };
  const next = jest.fn();

  const environment = {
    CID_REDIRECT_URI: 'mock cid redirect uri',
    CID_CLIENT_ID: 'mock cid client ID',
    CID_AUTH_ENDPOINT_URL: 'mock cid auth endpoint',
  };

  const callAuth = async (route) => {
    const to = route;
    if (to.query === undefined) {
      to.query = {
        referrer: 'UNTRUSTED_REFERRER',
      };
    }
    await auth({ store, to, next, router: $router });
  };

  beforeEach(() => {
    generateLoginUrlMock = jest.fn(() => ({ loginUrl: generatedLoginUrl }));
    AuthorisationService.mockImplementation(() => ({
      generateLoginUrl: generateLoginUrlMock,
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
      },
      $env: {
        SKIP_LOGGED_OUT_ENABLED: false,
      },
      dispatch: jest.fn(),
    };
    dependancy.createRouteByNameObject = jest.fn();
    dependancy.createRoutePathObject = jest.fn();
    dependancy.pathWithPatientPrefixOrUndefined.mockImplementation(x => `/patient/${x.path}`);
  });

  describe('isloggedIn is true', () => {
    const query = 'query';
    const params = 'param';
    beforeEach(async () => {
      getters['session/isLoggedIn'] = () => true;
      const to = {
        ...BOOKING,
        matched: [
          { ...BOOKING },
        ],
      };
      await callAuth(to);
    });

    it('will not be redirected', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });

    describe('when path is LOGIN', () => {
      beforeEach(async () => {
        const to = {
          ...LOGIN,
          query,
          params,
          store,
        };
        await callAuth(to);
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
      it('will not redirect if path is invalid', async () => {
        dependancy.pathWithPatientPrefixOrUndefined.mockImplementation(undefined);
        await callAuth(to);
        expect(dependancy.createRoutePathObject).not.toBeCalledWith(expect.anything);
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });

      it('will redirect if path is valid', async () => {
        await callAuth(to);
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

    describe('Audit referrer when value is valid', () => {
      const to = {
        meta: {},
        query: {
          referrer: 'NHS_UK',
        },
      };

      beforeEach(() => {
        next.mockClear();
        store.$env.SKIP_LOGGED_OUT_ENABLED = true;
        delete window.location;
        window.location = {};
      });

      describe('referrer is valid and is appended to path', () => {
        it('will skip login page and referrer value is retained', async () => {
          to.path = BOOKING.path;
          await callAuth(to);
          expect(generateLoginUrlMock).toBeCalledWith({
            cookies: store.$cookies,
            redirectTo: `/patient/${to.path}?integration_referrer=NHS_UK`,
            singleSignOnDetails: {
              assertedLoginIdentity: undefined,
              prompt: 'none',
            },
          });
          expect(window.location.href).toBe(generatedLoginUrl);
          expect(next).not.toBeCalled();
        });
      });
    });

    describe('Audit referrer when value is invalid', () => {
      const to = {
        meta: {},
        query: {
          referrer: 'unknown',
        },
      };

      beforeEach(() => {
        next.mockClear();
        store.$env.SKIP_LOGGED_OUT_ENABLED = true;
        delete window.location;
        window.location = {};
      });

      describe('referrer is invalid and path is valid', () => {
        it('will be redirected to logged out home page and referrer value is retained', async () => {
          to.path = BOOKING.path;
          await callAuth(to);
          const query = { [REDIRECT_PARAMETER]: `/patient/${BOOKING.path}?${INTEGRATION_REFERRER_PARAMETER}=${to.query.referrer}` };
          expect(next).toBeCalledWith({ name: LOGIN_NAME, query });
        });
      });

      describe('referrer is invalid and path is invalid', () => {
        it('will be redirected to logged out home page and referrer value is retained', async () => {
          to.path = 'invalid-path';
          dependancy.pathWithPatientPrefixOrUndefined.mockImplementation(undefined);
          await callAuth(to);
          const query = { [REDIRECT_PARAMETER]: `/?${INTEGRATION_REFERRER_PARAMETER}=${to.query.referrer}` };
          expect(next).toBeCalledWith({ name: LOGIN_NAME, query });
        });
      });

      describe('referrer is invalid and path is empty', () => {
        it('will be redirected to logged out home page and referrer value is retained', async () => {
          to.path = EMPTY_PATH;
          dependancy.pathWithPatientPrefixOrUndefined.mockImplementation(undefined);
          await callAuth(to);
          const query = { [REDIRECT_PARAMETER]: `/?${INTEGRATION_REFERRER_PARAMETER}=${to.query.referrer}` };
          expect(next).toBeCalledWith({ name: LOGIN_NAME, query });
        });
      });
    });

    describe('skip logged in page is true', () => {
      const to = {
        meta: {},
        query: {
          referrer: 'nhs_uk',
        },
      };

      beforeEach(() => {
        next.mockClear();
        store.$env.SKIP_LOGGED_OUT_ENABLED = true;

        delete window.location;
        window.location = {};
      });

      describe('referrer is valid and path is valid', () => {
        it('will skip the login page when the path is empty', async () => {
          to.path = EMPTY_PATH;
          dependancy.pathWithPatientPrefixOrUndefined.mockImplementation(x => `${x.path}`);
          await callAuth(to);

          expect(window.location.href).toBe(generatedLoginUrl);
          expect(next).not.toBeCalled();
        });
      });

      describe('referrer is valid but path is invalid', () => {
        it('will skip login page when the path is invalid', async () => {
          to.path = 'invalid-path';
          dependancy.pathWithPatientPrefixOrUndefined.mockImplementation(undefined);
          await callAuth(to);

          expect(window.location.href).toBe(generatedLoginUrl);
          expect(next).not.toBeCalled();
        });
      });
    });

    describe('when is a known route', () => {
      beforeEach(async () => {
        await callAuth(BOOKING);
      });

      it('will be redirected to the login page with redirecturl query param', () => {
        const query = { [REDIRECT_PARAMETER]: BOOKING.name };
        expect(next).toBeCalledWith({ name: LOGIN_NAME, query });
      });
    });

    describe('when route is an empty path', () => {
      beforeEach(async () => {
        await callAuth({ path: EMPTY_PATH });
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME });
      });
    });

    describe('when route has no name', () => {
      beforeEach(async () => {
        await callAuth({ path: 'some_path' });
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME });
      });
    });

    describe('when route is the redirector page', () => {
      beforeEach(async () => {
        await callAuth(REDIRECTOR);
      });

      it('will be redirected to the login page without a redirecturl query param', () => {
        expect(next).toBeCalledWith({ name: LOGIN_NAME });
      });
    });

    describe('when route is the redirector page with a redirect parameter', () => {
      const query = {};
      query[REDIRECT_PARAMETER] = APPOINTMENTS_NAME;
      beforeEach(async () => {
        await callAuth({
          ...{ path: 'redirector' },
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

      beforeEach(async () => {
        store.$env = environment;
        getters['session/isLoggedIn'] = () => false;

        delete window.location;
        window.location = {
          hostname,
        };

        await callAuth({ ...BEGIN_LOGIN,
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
