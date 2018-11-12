import actions from '@/store/modules/session/actions';
import NativeCallbacks from '@/services/native-app';

import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SHOW_SESSION_EXPIRING,
  HIDE_SESSION_EXPIRING,
} from '@/store/modules/session/mutation-types';

describe('actions', () => {
  let mutation;

  beforeEach(() => {
    actions.app = {
      $cookies: {
        get: jest.fn(),
        set: jest.fn(),
        remove: jest.fn(),
      },
      $env: jest.fn(),
    };
    mutation = {
      commit: jest.fn(),
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
    it('will have a setInfo function', () => expect(actions.setInfo).toBeInstanceOf(Function));

    it('will call commit for the SET_INFO mutation passing the info object', () => {
      const input = {};

      actions.setInfo(mutation, input);

      expect(mutation.commit).toHaveBeenCalledWith(SET_INFO, input);
    });
  });

  describe('validation checking', () => {
    beforeEach(() => {
      process.client = true;
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
          process.client = false;
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

  describe('update last called at', () => {
    it('will have a updateLastCalledAt function', () => {
      expect(actions.updateLastCalledAt).toBeInstanceOf(Function);
    });

    it(
      'will call commit for the SET_LAST_CALLED_AT mutation passing the current date and time',
      () => {
        const now = new Date();
        actions.updateLastCalledAt(mutation, now);
        expect(mutation.commit.mock.calls[0][0]).toEqual(SET_LAST_CALLED_AT);
        expect(mutation.commit.mock.calls[0][1] - now).toBeCloseTo(0);
      },
    );

    it(
      'will call commit for the HIDE_SESSION_EXPIRING mutation',
      () => {
        actions.updateLastCalledAt(mutation, new Date());
        expect(mutation.commit.mock.calls[1][0]).toEqual(HIDE_SESSION_EXPIRING);
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
      app = {
        dispatch: jest.fn(),
        validate: actions.validate,
        app: {
          $env: {
            SESSION_EXPIRING_WARNING_SECONDS: 10,
          },
        },
      };
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

        spy = jest.spyOn(NativeCallbacks, 'onSessionExpiring').mockImplementation(() => true);
      });

      afterEach(() => {
        (spy || {}).mockRestore();
        window.nativeApp = undefined;
      });

      it('will not call global dispatch', () => {
        app.validate(store);
        expect(app.dispatch).not.toHaveBeenCalled();
      });

      it('will return true', () => {
        const result = app.validate(store);
        expect(result).toEqual(true);
      });

      it('will not call native onSessionExpiring callback if not native', () => {
        window.nativeApp = undefined;

        app.validate(store);
        expect(NativeCallbacks.onSessionExpiring).not.toBeCalled();
      });

      it('will not call native onSessionExpiring callback if not expiring', () => {
        store.getters.isExpiring = () => false;

        app.validate(store);
        expect(NativeCallbacks.onSessionExpiring).not.toBeCalled();
      });

      it('will call native onSessionExpiring callback if expiring and native', () => {
        app.validate(store);
        expect(NativeCallbacks.onSessionExpiring).toHaveBeenCalledTimes(1);
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
  });
});
