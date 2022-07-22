import biometricRegistrationPrompt from '@/middleware/biometricRegistrationPrompt';
import { BIOMETRICS_REGISTRATION_PATH } from '@/router/paths';
import { INDEX_NAME, BIOMETRICS_REGISTRATION_NAME } from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

describe('biometrics prompt next', () => {
  const params = 'params';
  const query = 'query';
  let isNativeApp;
  let next;
  let store;

  const callsBiometricsPrompt = ({ biometricsCookieExists = false, BIOMETRICS_REGISTRATION_ENABLED = true } = {}) => {
    const to = {
      name: BIOMETRICS_REGISTRATION_NAME,
      params,
      path: BIOMETRICS_REGISTRATION_PATH,
      query,
    };
    next = jest.fn();
    store = createStore({
      getters: {
        'session/isLoggedIn': jest.fn().mockReturnValue(true),
      },
      state: {
        device: {
          isNativeApp,
        },
        biometrics: {
          biometricsCookieExists,
        },
      },
      $env: {
        BIOMETRICS_REGISTRATION_ENABLED,
      },
    });
    biometricRegistrationPrompt({ next, to, store });
  };

  describe('native', () => {
    const redirectRoute = 'redirectRoute';
    beforeEach(() => {
      isNativeApp = true;
    });
    describe('biometrics cookie exists', () => {
      beforeEach(() => {
        callsBiometricsPrompt({ biometricsCookieExists: true, BIOMETRICS_REGISTRATION_ENABLED: true });
        createConditionalRedirectRouteByName.mockReturnValue(redirectRoute);
      });

      it('will dispatch `biometrics/checkBiometricsCookie`', () => {
        expect(store.dispatch).toBeCalledWith('biometrics/checkBiometricsCookie');
      });

      it('will call next', () => {
        expect(next).toBeCalledWith(redirectRoute);
      });
    });

    describe('biometrics cookie does not exist', () => {
      beforeEach(() => {
        callsBiometricsPrompt({ biometricsCookieExists: false, BIOMETRICS_REGISTRATION_ENABLED: true });
      });

      it('will dispatch `biometrics/checkBiometricsCookie`', () => {
        expect(store.dispatch).toBeCalledWith('biometrics/checkBiometricsCookie');
      });

      it('will call next', () => {
        expect(next).toBeCalled();
      });
    });

    describe('biometrcis not enabled', () => {
      beforeEach(() => {
        callsBiometricsPrompt({ BIOMETRICS_REGISTRATION_ENABLED: false });
        createConditionalRedirectRouteByName.mockReturnValue(redirectRoute);
      });

      it('will not dispatch any actions', () => {
        expect(store.dispatch).not.toBeCalled();
      });

      it('will create redirect route to INDEX', () => {
        expect(createConditionalRedirectRouteByName).toBeCalledWith({
          name: INDEX_NAME,
          params,
          query,
          store,
        });
      });

      it('will call next with redirect route', () => {
        expect(next).toBeCalledWith(redirectRoute);
      });
    });
  });

  describe('not on native platform', () => {
    const redirectRoute = 'redirectRoute';

    beforeEach(() => {
      isNativeApp = false;
      createConditionalRedirectRouteByName.mockReturnValue(redirectRoute);
      callsBiometricsPrompt({ isNativeApp: false });
    });

    it('will not dispatch any actions', () => {
      expect(store.dispatch).not.toBeCalled();
    });

    it('will create redirect route to INDEX', () => {
      expect(createConditionalRedirectRouteByName).toBeCalledWith({
        name: INDEX_NAME,
        params,
        query,
        store,
      });
    });

    it('will call next with redirect route', () => {
      expect(next).toBeCalledWith(redirectRoute);
    });
  });

  afterEach(() => {
    isNativeApp = undefined;
  });
});
