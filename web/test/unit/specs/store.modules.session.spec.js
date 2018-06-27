import moment from 'moment';
import session from '../../../src/store/modules/session';

import {
  CLEAR,
  HIDE_EXPIRY_MESSAGE,
  SET_DURATION_SECONDS,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
} from '../../../src/store/modules/session/mutation-types';

const {
  actions,
  getters,
  mutations,
  state,
} = session;

describe('actions', () => {
  let mutation;
  const {
    clear,
    hideExpiryMessage,
    setDurationSeconds,
    showExpiryMessage,
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

  describe('set duration seconds at', () => {
    it('will have a setDurationSeconds function', () => expect(setDurationSeconds).toBeInstanceOf(Function));

    it('will call commit for the SET_DURATION_SECONDS mutation passing the received number of seconds', () => {
      const input = 25;

      setDurationSeconds(mutation, input);

      expect(mutation.commit).toHaveBeenCalledWith(SET_DURATION_SECONDS, input);
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

      it('will return false', () => {
        const result = app.validate(store);
        expect(result).toEqual(false);
      });
    });
  });
});

describe('getters', () => {
  describe('isValid', () => {
    const { isValid } = getters;

    it('will be false if lastCalledAt is not set', () => {
      const currentState = { durationSeconds: 430 };
      expect(isValid(currentState)()).toEqual(false);
    });

    it('will be false if durationSeconds is not set', () => {
      const currentState = { lastCalledAt: new Date() };

      expect(isValid(currentState)()).toEqual(false);
    });

    it('will be false if now is greater than lastCalledAt plus durationSeconds', () => {
      const now = moment();
      const durationSeconds = 5000;
      const lastCalledAt = now.subtract(durationSeconds + 1000, 'seconds');
      const currentState = { durationSeconds, lastCalledAt };

      expect(isValid(currentState)()).toEqual(false);
    });

    it('will be true if now is less than lastCalledAt plus durationSeconds', () => {
      const now = moment();
      const durationSeconds = 5000;
      const lastCalledAt = now.toDate();
      const currentState = { durationSeconds, lastCalledAt };

      expect(isValid(currentState)()).toEqual(true);
    });
  });
});

describe('mutations', () => {
  describe('CLEAR', () => {
    it('will set lastCalledAt to undefined', () => {
      const stateToMutate = { lastCalledAt: new Date() };
      mutations[CLEAR](stateToMutate);
      expect(stateToMutate.lastCalledAt).toBeUndefined();
    });

    it('will set durationSeconds to undefined', () => {
      const stateToMutate = { durationSeconds: 15 };
      mutations[CLEAR](stateToMutate);
      expect(stateToMutate.durationSeconds).toBeUndefined();
    });

    it('will set durationSeconds to undefined', () => {
      const stateToMutate = { lastCalledAt: new Date() };
      mutations[CLEAR](stateToMutate);
      expect(stateToMutate.durationSeconds).toBeUndefined();
    });
  });

  describe('SET_LAST_CALLED_AT', () => {
    it('will set the lastCalledAt on the state', () => {
      const stateToMutation = {};
      const expected = new Date(2018, 4, 4);

      mutations[SET_LAST_CALLED_AT](stateToMutation, expected);
      expect(stateToMutation.lastCalledAt).toBe(expected);
    });
  });

  describe('SET_DURATION_SECONDS', () => {
    it('will set the durationSeconds on the state', () => {
      const stateToMutate = {};
      const expected = 2300;
      mutations[SET_DURATION_SECONDS](stateToMutate, expected);
      expect(stateToMutate.durationSeconds).toBe(expected);
    });
  });

  describe('SHOW_EXPIRY_MESSAGE', () => {
    it('will set the showExpiryMessage on the state to true', () => {
      const stateToMutate = {};
      mutations[SHOW_EXPIRY_MESSAGE](stateToMutate);
      expect(stateToMutate.showExpiryMessage).toBe(true);
    });
  });

  describe('HIDE_EXPIRY_MESSAGE', () => {
    it('will set the remove the showExpiryMessage property on the state', () => {
      const stateToMutate = {
        showExpiryMessage: true,
      };

      mutations[HIDE_EXPIRY_MESSAGE](stateToMutate);
      expect(stateToMutate.showExpiryMessage).toBeUndefined();
      expect(stateToMutate).not.toHaveProperty('showExpiryMessage');
    });
  });
});

describe('state', () => {
  describe('initial state', () => {
    it('will have a lastCalledAt property', () => {
      expect(state()).toHaveProperty('lastCalledAt');
    });

    it('will have a durationSeconds property', () => {
      expect(state()).toHaveProperty('durationSeconds');
    });

    it('will have lastCalledAt set to undefined', () => {
      expect(state().lastCalledAt).toEqual(undefined);
    });

    it('will have durationSeconds set to undefined', () => {
      expect(state().durationSeconds).toEqual(undefined);
    });
  });
});
