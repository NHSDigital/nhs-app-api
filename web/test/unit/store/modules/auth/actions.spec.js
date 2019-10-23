import each from 'jest-each';
import actions from '@/store/modules/auth/actions';
import { AUTH_RESPONSE, LOGOUT, UPDATE_CONFIG } from '@/store/modules/auth/mutation-types';
import NativeCallbacks from '@/services/native-app';
import { mockCookies } from '../../../helpers';
import Sources from '../../../../../src/lib/sources';

describe('actions', () => {
  const name = 'Montel';
  const odsCode = 'A12345';
  const sessionTimeout = 1200;
  const token = 'sdfdhgmbnrdstgjxjcbv';
  const postSessionResponse = {
    odsCode,
    name,
    sessionTimeout,
    token,
  };
  let commit;
  let state;

  const finalAsserts = () => {
    it('will commit the value `true` to `LOGOUT`', () => {
      expect(commit).toBeCalledWith(LOGOUT, true);
    });

    each([
      'analytics/init',
      'appVersion/init',
      'auth/init',
      'availableAppointments/init',
      'device/init',
      'errors/clearAllApiErrors',
      'flashMessage/init',
      'header/init',
      'http/init',
      'messaging/init',
      'myAppointments/init',
      'myRecord/init',
      'navigation/init',
      'organDonation/init',
      'repeatPrescriptionCourses/init',
      'serviceJourneyRules/init',
      'session/setInfo',
      'termsAndConditions/init',
    ]).it('will dispatch the `%s` event', (action) => {
      expect(actions.dispatch).toHaveBeenCalledWith(action);
    });
  };

  const removeSessionCookiesAsserts = () => {
    it('will remove the `nhso.session` cookie', () => {
      expect(actions.app.$cookies.remove).toHaveBeenCalledWith('nhso.session');
    });

    it('will remove the `nhso.terms` cookie', () => {
      expect(actions.app.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });

    it('will remove the `NHSO-Session-Id` cookie', () => {
      expect(actions.app.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });
  };

  const logoutCleanUpAsserts = () => {
    each([
      'session/clear',
      'session/endValidationChecking',
      'errors/disableApiError',
      'navigation/clearPreviousSelectedMenuItem',
    ]).it('will dispatch the `%s` event', (action) => {
      expect(actions.dispatch).toHaveBeenCalledWith(action);
    });

    removeSessionCookiesAsserts();
  };

  beforeEach(() => {
    actions.app = {
      $http: {
        postV1Session: jest.fn(() => Promise.resolve(postSessionResponse)),
        deleteV1Session: jest.fn(() => Promise.resolve()),
      },
      $cookies: mockCookies(),
      router: {
        go: jest.fn(),
        push: jest.fn(),
      },
      store: {
        state: {
          device: { source: Sources.Web },
        },
      },
      context: {
        redirect: jest.fn(),
      },
    };

    commit = jest.fn();
    state = {
      config: {},
      session: {},
    };

    actions.dispatch = jest.fn();
    actions.state = state;
  });

  describe('handle auth response', () => {
    beforeEach(async () => {
      await actions.handleAuthResponse({ commit, state }, '123');
    });

    it('will set the session info from the received session timeout and the response', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/setInfo', {
        name,
        durationSeconds: sessionTimeout,
        token,
        gpOdsCode: odsCode,
      });
    });

    it('will hide start validation checking', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/hideExpiryMessage');
    });

    it('will hide the expiry message', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/startValidationChecking');
    });

    it('will remove the "nhso.auth" cookie', () => {
      expect(actions.app.$cookies.remove).toHaveBeenCalledWith('nhso.auth');
    });

    it('will commit the AUTH_RESPONSE', () => {
      expect(commit).toHaveBeenCalledWith(AUTH_RESPONSE, postSessionResponse);
    });
  });

  describe('logout', () => {
    beforeEach(async () => {
      await actions.logout({ commit });
    });

    logoutCleanUpAsserts();

    removeSessionCookiesAsserts();

    finalAsserts();
  });

  describe('logoutNoJs', () => {
    beforeEach(async () => {
      await actions.logoutNoJs();
    });

    removeSessionCookiesAsserts();
  });

  describe('logoutWhenExpired', () => {
    beforeEach(() => {
      actions.logoutWhenExpired();
    });

    each([
      'session/showExpiryMessage',
      'modal/hide',

    ]).it('will dispatch the `%s` event', (action) => {
      expect(actions.dispatch).toHaveBeenCalledWith(action);
    });

    it('will dispatch the `auth/logout` event', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('auth/logout', { expired: true });
    });
  });

  describe('updateConfig', () => {
    it('will call commit with the sent value', () => {
      const newConfigValue = { test: 'value' };

      actions.updateConfig({ commit }, newConfigValue);

      expect(commit).toBeCalledWith(UPDATE_CONFIG, newConfigValue);
    });
  });

  describe('nativeLogin', () => {
    let spy;

    beforeEach(() => {
      jest.useFakeTimers();
      window.nativeApp = undefined;
    });

    afterEach(() => {
      (spy || {}).mockRestore();
    });

    it('will attempt to fire the native onLogin callback and fail as the app has timed out', () => {
      process.server = false;
      spy = jest.spyOn(NativeCallbacks, 'onLogin').mockImplementation(() => false);

      actions.nativeLogin();

      // Fast-forward until all timers have been executed
      jest.runTimersToTime(10000);

      expect(NativeCallbacks.onLogin).toHaveBeenCalledTimes(11);
    });


    it('will attempt to fire the native onLogin callback and succeed ', () => {
      process.server = false;

      let attempts = 0;
      spy = jest.spyOn(NativeCallbacks, 'onLogin').mockImplementation(() => {
        attempts += 1;
        return attempts > 1;
      });

      actions.nativeLogin();

      // Fast-forward until all timers have been executed
      jest.runTimersToTime(10000);

      expect(NativeCallbacks.onLogin).toHaveBeenCalledTimes(2);
    });
  });

  describe('unauthorised', () => {
    beforeEach(() => {
      actions.unauthorised({ commit });
    });

    logoutCleanUpAsserts();

    finalAsserts();
  });
});
