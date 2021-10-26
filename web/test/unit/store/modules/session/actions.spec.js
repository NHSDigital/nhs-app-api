import actions from '@/store/modules/session/actions';
import NativeApp from '@/services/native-app';
import SessionExpiryModal from '@/components/modal/content/SessionExpiryModal';
import { isAnonymous } from '@/router';
import { APPOINTMENTS_NAME } from '@/router/names';
import { setCookie } from '@/lib/cookie-manager';

import {
  CLEAR,
  END_VALIDATION_CHECKING,
  LOGOUT,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SHOW_SESSION_EXPIRING,
  HIDE_SESSION_EXPIRING,
} from '@/store/modules/session/mutation-types';

import { createRouter } from '../../../helpers';

jest.mock('@/services/native-app');
jest.mock('@/lib/cookie-manager');
jest.mock('@/services/native-app');
jest.mock('@/router');

describe('actions', () => {
  let mutation;
  let $router;
  const $cookies = {
    get: jest.fn(),
    set: jest.fn(),
    remove: jest.fn(),
  };
  const $env = {
    SECURE_COOKIES: true,
    XAMARIN_INITIAL_RELEASE_VERSION: '1.99.0',
  };

  beforeEach(() => {
    actions.$cookies = $cookies;
    actions.$env = $env;
    mutation = {
      commit: jest.fn(),
      dispatch: jest.fn(),
      state: { },
    };
  });

  describe('expiry message', () => {
    it('will have an hideExpiryMessage function', () => expect(actions.hideExpiryMessage).toBeInstanceOf(Function));
    it('will have an showExpiryMessage function', () => expect(actions.showExpiryMessage).toBeInstanceOf(Function));

    it('will call commit for the HIDE_EXPIRY_MESSAGE mutation', () => {
      actions.hideExpiryMessage(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(HIDE_EXPIRY_MESSAGE);
    });

    it('will call commit for the SHOW_EXPIRY_MESSAGE mutation', () => {
      actions.showExpiryMessage(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(SHOW_EXPIRY_MESSAGE);
    });
  });

  describe('set info', () => {
    let userAgentGetter;
    beforeEach(() => {
      delete window.location;
      window.location = new URL('https://www.example.com');
      userAgentGetter = jest.spyOn(window.navigator, 'userAgent', 'get');
    });

    describe('Native app version in the userAgent is less than xamarin release version', () => {
      beforeEach(() => {
        userAgentGetter.mockReturnValue('Mozilla/5.0 (iPhone; CPU iPhone OS 14_3 like Mac OS X) AppleWebKit/605.1.15 (KTML, like Gecko) nhsapp-ios/1.89.0 nhsapp-manufacturer/Apple nhsapp-model/x86_64 nhsapp-os/14.3 nhsapp-architecture/x64');
      });

      it('will have a setInfo function', () => expect(actions.setInfo).toBeInstanceOf(Function));

      it('will call commit for the SET_INFO mutation passing the info object', () => {
        const input = {};

        actions.setInfo(mutation, input);
        expect(mutation.commit).toHaveBeenCalledWith(SET_INFO, input);
      });

      it('will set the cookie with the new values and no explicit domain', () => {
        const input = {};
        actions.setInfo(mutation, input);

        expect(setCookie).toHaveBeenCalledWith({
          key: 'nhso.session',
          value: input,
          cookies: $cookies,
          secure: $env.SECURE_COOKIES,
        });
      });
    });

    describe('Native app version in the userAgent is greater than xamarin release version', () => {
      beforeEach(() => {
        userAgentGetter.mockReturnValue('Mozilla/5.0 (iPhone; CPU iPhone OS 14_3 like Mac OS X) AppleWebKit/605.1.15 (KTML, like Gecko) nhsapp-ios/1.99.1 nhsapp-manufacturer/Apple nhsapp-model/x86_64 nhsapp-os/14.3 nhsapp-architecture/x64');
      });

      it('will have a setInfo function', () => expect(actions.setInfo).toBeInstanceOf(Function));

      it('will call commit for the SET_INFO mutation passing the info object', () => {
        const input = {};

        actions.setInfo(mutation, input);
        expect(mutation.commit).toHaveBeenCalledWith(SET_INFO, input);
      });

      it('will set the cookie with the new values and no explicit domain', () => {
        const input = {};
        actions.setInfo(mutation, input);

        expect(setCookie).toHaveBeenCalledWith({
          key: 'nhso.session',
          value: input,
          cookies: $cookies,
          secure: $env.SECURE_COOKIES,
          domain: '.www.example.com',
        });
      });
    });

    describe('Native app version in the userAgent is equal to xamarin release version', () => {
      beforeEach(() => {
        userAgentGetter.mockReturnValue('Mozilla/5.0 (iPhone; CPU iPhone OS 14_3 like Mac OS X) AppleWebKit/605.1.15 (KTML, like Gecko) nhsapp-ios/1.99.0 nhsapp-manufacturer/Apple nhsapp-model/x86_64 nhsapp-os/14.3 nhsapp-architecture/x64');
      });

      it('will have a setInfo function', () => expect(actions.setInfo).toBeInstanceOf(Function));

      it('will call commit for the SET_INFO mutation passing the info object', () => {
        const input = {};

        actions.setInfo(mutation, input);
        expect(mutation.commit).toHaveBeenCalledWith(SET_INFO, input);
      });

      it('will set the cookie with the new values and no explicit domain', () => {
        const input = {};
        actions.setInfo(mutation, input);

        expect(setCookie).toHaveBeenCalledWith({
          key: 'nhso.session',
          value: input,
          cookies: $cookies,
          secure: $env.SECURE_COOKIES,
          domain: '.www.example.com',
        });
      });

      describe('No native app version in the userAgent', () => {
        beforeEach(() => {
          userAgentGetter.mockReturnValue('Mozilla/5.0 (iPhone; CPU iPhone OS 14_3 like Mac OS X) AppleWebKit/605.1.15 (KTML, like Gecko) nhsapp-manufacturer/Apple nhsapp-model/x86_64 nhsapp-os/14.3 nhsapp-architecture/x64');
        });

        it('will have a setInfo function', () => expect(actions.setInfo).toBeInstanceOf(Function));

        it('will call commit for the SET_INFO mutation passing the info object', () => {
          const input = {};

          actions.setInfo(mutation, input);
          expect(mutation.commit).toHaveBeenCalledWith(SET_INFO, input);
        });

        it('will set the cookie with the new values and no explicit domain', () => {
          const input = {};
          actions.setInfo(mutation, input);

          expect(setCookie).toHaveBeenCalledWith({
            key: 'nhso.session',
            value: input,
            cookies: $cookies,
            secure: $env.SECURE_COOKIES,
          });
        });
      });

      afterEach(() => {
        userAgentGetter.mockClear();
      });
    });
  });

  describe('validation checking', () => {
    beforeEach(() => {
      mutation.state = {};
      mutation.getters = {
        isLoggedIn: () => true,
      };
    });

    describe('start validation checking', () => {
      it('will have a startValidationChecking function', () =>
        expect(actions.startValidationChecking).toBeInstanceOf(Function));

      it(
        'will call commit for the START_VALIDATION_CHECKING passing an interval when there is no validationInterval',
        () => {
          actions.startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0][0]).toEqual(START_VALIDATION_CHECKING);
          expect(mutation.commit.mock.calls[0][1]).toBeGreaterThan(0);
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING there is a validationInterval',
        () => {
          mutation.state.validationInterval = 1;
          actions.startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING when not running on the server',
        () => {
          mutation.state.validationInterval = 1;
          actions.startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING not logged in',
        () => {
          mutation.state.validationInterval = 1;
          mutation.state.csrfToken = undefined;
          actions.startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );
    });

    describe('end validation checking', () => {
      it(
        'will have a endValidationChecking function',
        () => expect(actions.endValidationChecking).toBeInstanceOf(Function),
      );

      it('will call commit for the END_VALIDATION_CHECKING mutation', () => {
        actions.endValidationChecking(mutation);
        expect(mutation.commit.mock.calls[0][0]).toEqual(END_VALIDATION_CHECKING);
      });

      it('will call commit for the HIDE_SESSION_EXPIRING mutation', () => {
        actions.endValidationChecking(mutation);
        expect(mutation.commit.mock.calls[1][0]).toEqual(HIDE_SESSION_EXPIRING);
      });
    });
  });

  describe('logout', () => {
    it('will have a logout function', () => {
      expect(actions.logout).toBeInstanceOf(Function);
    });

    it(
      'will call commit for the LOGOUT mutation passing the current date and time',
      () => {
        actions.logout(mutation);
        expect(mutation.commit).toHaveBeenCalledWith(LOGOUT);
      },
    );
  });

  describe('update last called at', () => {
    it('will have a updateLastCalledAt function', () => {
      expect(actions.updateLastCalledAt).toBeInstanceOf(Function);
    });

    it(
      'will call commit for the SET_LAST_CALLED_AT mutation passing the current date and time',
      () => {
        const now = new Date();
        actions.updateLastCalledAt(mutation, now);
        expect(mutation.dispatch).toHaveBeenCalledWith('setInfo', { lastCalledAt: now });
      },
    );

    it(
      'will call commit for the HIDE_SESSION_EXPIRING mutation',
      () => {
        actions.updateLastCalledAt(mutation, new Date());
        expect(mutation.commit.mock.calls[0][0]).toEqual(HIDE_SESSION_EXPIRING);
      },
    );
  });

  describe('clear', () => {
    it('will have a clear function', () => {
      expect(actions.clear).toBeInstanceOf(Function);
    });

    it('will call commit for the CLEAR mutation', () => {
      actions.clear(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(CLEAR);
    });
  });

  describe('validate session', () => {
    let app;
    let store;

    beforeEach(() => {
      $router = createRouter(APPOINTMENTS_NAME);

      app = {
        dispatch: jest.fn(),
        validate: actions.validate,
        $env: {
          SESSION_EXPIRING_WARNING_SECONDS: 10,
        },
        $router,
      };

      actions.app = app;

      store = {
        getters: {
          isValid: () => true,
        },
        commit: jest.fn(),
        state: {
          durationSeconds: 60,
        },
      };
    });

    it('will have a validate function', () => {
      expect(app.validate).toBeInstanceOf(Function);
    });

    describe('is valid', () => {
      let spy;

      beforeEach(() => {
        store.getters.isLoggedIn = () => true;
        store.getters.isExpiring = () => true;
        window.nativeApp = true;

        spy = jest.spyOn(NativeApp, 'onSessionExpiring').mockImplementation(() => true);
      });

      afterEach(() => {
        (spy || {}).mockRestore();
        window.nativeApp = undefined;
      });

      it('will call dispatch to stayOnPage for pageLeaveWarning', () => {
        app.validate(store);
        expect(app.dispatch).toHaveBeenCalledWith('pageLeaveWarning/stayOnPage');
      });

      it('will return true', () => {
        const result = app.validate(store);
        expect(result).toEqual(true);
      });

      it('will not call native onSessionExpiring callback if not native', () => {
        window.nativeApp = undefined;

        app.validate(store);
        expect(NativeApp.onSessionExpiring).not.toBeCalled();
      });

      it('will not call native onSessionExpiring callback if not expiring', () => {
        store.getters.isExpiring = () => false;

        app.validate(store);
        expect(NativeApp.onSessionExpiring).not.toBeCalled();
      });

      it('will call native onSessionExpiring callback if expiring and native', () => {
        app.validate(store);
        expect(NativeApp.onSessionExpiring).toHaveBeenCalledTimes(1);
      });

      it('will call dispatch moal/show  if expiring and desktop', () => {
        window.nativeApp = undefined;

        app.validate(store);
        expect(app.dispatch).toHaveBeenCalledWith('modal/show', { content: SessionExpiryModal });
      });

      it('will call commit for the SHOW_SESSION_EXPIRING mutation if expiring and native', () => {
        app.validate(store);
        expect(store.commit.mock.calls[0][0]).toEqual(SHOW_SESSION_EXPIRING);
      });
    });

    describe('is not valid', () => {
      beforeEach(() => {
        store.getters.isValid = () => false;
        store.getters.isLoggedIn = () => true;
      });

      it('will call global dispatch with the logout action', () => {
        app.validate(store);
        expect(app.dispatch).toHaveBeenCalledWith('auth/logoutWhenExpired');
      });

      it('will return false', () => {
        const result = app.validate(store);
        expect(result).toEqual(false);
      });
    });

    describe('is not logged in', () => {
      describe('is not native', () => {
        beforeEach(() => {
          store.getters.isLoggedIn = () => false;
        });

        it('will not call global dispatch with the logout action', () => {
          app.validate(store);
          expect(app.dispatch).not.toHaveBeenCalledWith('auth/logoutWhenExpired');
        });

        it('will return false', () => {
          const result = app.validate(store);
          expect(result).toEqual(false);
        });
      });

      describe('is native', () => {
        beforeEach(() => {
          store.getters.isLoggedIn = () => false;
          isAnonymous.mockReturnValue(false);

          NativeApp.supportsLogout.mockReturnValue(true);
          actions.dispatch = jest.fn();

          actions.app = app;
        });

        it('will call global dispatch with the logout action', () => {
          actions.validate(store);
          expect(actions.dispatch).toHaveBeenCalledWith('auth/logoutNativeWhenAlreadyExpired');
        });

        it('will return false', () => {
          const result = actions.validate(store);
          expect(result).toEqual(false);
        });
      });
    });
  });
});
