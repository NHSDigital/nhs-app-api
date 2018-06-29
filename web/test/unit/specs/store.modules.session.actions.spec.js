import actions from '../../../src/store/modules/session/actions';

import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_DURATION_SECONDS,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
} from '../../../src/store/modules/session/mutation-types';

describe('actions', () => {
  let mutation;
  const {
    clear,
    endValidationChecking,
    hideExpiryMessage,
    setDurationSeconds,
    showExpiryMessage,
    startValidationChecking,
    updateLastCalledAt,
    validate,
  } = actions;

  beforeEach(() => {
    mutation = { commit: jest.fn() };
  });

  describe('expiry message', () => {
    it('will have an hideExpiryMessage function', () => expect(hideExpiryMessage).toBeInstanceOf(Function));
    it('will have an showExpiryMessage function', () => expect(showExpiryMessage).toBeInstanceOf(Function));

    it('will call commit for the HIDE_EXPIRY_MESSAGE mutation', () => {
      hideExpiryMessage(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(HIDE_EXPIRY_MESSAGE);
    });

    it('will call commit for the SHOW_EXPIRY_MESSAGE mutation', () => {
      showExpiryMessage(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(SHOW_EXPIRY_MESSAGE);
    });
  });

  describe('set duration seconds', () => {
    it('will have a setDurationSeconds function', () => expect(setDurationSeconds).toBeInstanceOf(Function));

    it('will call commit for the SET_DURATION_SECONDS mutation passing the received number of seconds', () => {
      const input = 25;

      setDurationSeconds(mutation, input);

      expect(mutation.commit).toHaveBeenCalledWith(SET_DURATION_SECONDS, input);
    });
  });

  describe('validation checking', () => {
    beforeEach(() => {
      process.client = true;
      mutation.state = { };
      mutation.rootState = {
        auth: { loggedIn: true },
      };
    });

    describe('start validation checking', () => {
      it('will have a startValidationChecking function', () => expect(startValidationChecking).toBeInstanceOf(Function));
      it(
        'will call commit for the START_VALIDATION_CHECKING passing an interval when there is no validationInterval',
        () => {
          startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0][0]).toEqual(START_VALIDATION_CHECKING);
          expect(mutation.commit.mock.calls[0][1]).toBeGreaterThan(0);
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING there is a validationInterval',
        () => {
          mutation.state.validationInterval = 1;
          startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING when not running on the client',
        () => {
          mutation.state.validationInterval = 1;
          process.client = false;
          startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING not logged in',
        () => {
          mutation.state.validationInterval = 1;
          mutation.rootState.auth.loggedIn = false;
          startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );
    });

    describe('end validation checking', () => {
      it('will have a endValidationChecking function', () => expect(endValidationChecking).toBeInstanceOf(Function));
      it('will call commit for the END_VALIDATION_CHECKING mutation', () => {
        endValidationChecking(mutation);
        expect(mutation.commit.mock.calls[0][0]).toEqual(END_VALIDATION_CHECKING);
      });
    });
  });

  describe('update last called at', () => {
    it('will have a updateLastCalledAt function', () => {
      expect(updateLastCalledAt).toBeInstanceOf(Function);
    });

    it('will call commit for the SET_LAST_CALLED_AT mutation passing the current date and time', () => {
      const now = new Date();
      updateLastCalledAt(mutation, now);
      expect(mutation.commit.mock.calls[0][0]).toEqual(SET_LAST_CALLED_AT);
      expect(mutation.commit.mock.calls[0][1] - now).toBeCloseTo(0);
    });
  });

  describe('clear', () => {
    it('will have a clear function', () => {
      expect(clear).toBeInstanceOf(Function);
    });

    it('will call commit for the CLEAR mutation', () => {
      clear(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(CLEAR);
    });
  });

  describe('validate session', () => {
    let app;
    let store;

    beforeEach(() => {
      app = {
        dispatch: jest.fn(),
        validate,
      };
      store = {
        getters: {
          isValid: () => true,
        },
        commit: jest.fn(),
      };
    });

    it('will have a validate function', () => {
      expect(app.validate).toBeInstanceOf(Function);
    });

    describe('is valid', () => {
      beforeEach(() => {
        store.getters.isValid = () => true;
      });

      it('will not call global dispatch', () => {
        app.validate(store);
        expect(app.dispatch).not.toHaveBeenCalled();
      });

      it('will return true', () => {
        const result = app.validate(store);
        expect(result).toEqual(true);
      });
    });

    describe('is not valid', () => {
      beforeEach(() => {
        store.getters.isValid = () => false;
      });

      it('will call global dispatch with the logout action', () => {
        app.validate(store);
        expect(app.dispatch).toHaveBeenCalledWith('auth/logoutWhenExpired');
      });

      it('will call the endValidationChecking action', () => {
        app.validate(store);
        expect(app.dispatch).toHaveBeenCalledWith('session/endValidationChecking');
      });

      it('will return false', () => {
        const result = app.validate(store);
        expect(result).toEqual(false);
      });
    });
  });
});
