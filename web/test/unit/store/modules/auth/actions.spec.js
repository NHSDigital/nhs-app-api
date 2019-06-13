import actions from '@/store/modules/auth/actions';
import { AUTH_RESPONSE, UPDATE_CONFIG } from '@/store/modules/auth/mutation-types';
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

    it('will dispatch the session/clear event', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/clear');
    });

    it('will dispatch the session/endValidationChecking event', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/endValidationChecking');
    });

    it('will clear the info token by dispatching the session/setInfo event with no parameters', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/setInfo');
    });

    it('will remove the nhso.session cookie', () => {
      expect(actions.app.$cookies.remove).toHaveBeenCalledWith('nhso.session');
    });

    it('will remove the nhso.terms cookie', () => {
      expect(actions.app.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });

    it('will dispatch serviceJourneyRules/init', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('serviceJourneyRules/init');
    });
  });

  describe('logoutWhenExpired', () => {
    it('will dispatch the session/showExpiryMessage and auth/logout event', () => {
      actions
        .logoutWhenExpired();

      expect(actions.dispatch).toHaveBeenCalledWith('session/showExpiryMessage');
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

    it('will dispatch serviceJourneyRules/init', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('serviceJourneyRules/init');
    });
  });
});
