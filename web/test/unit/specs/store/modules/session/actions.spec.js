import actions from '@/store/modules/session/actions';

import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
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
      mutation.state = {
        csrfToken: 'logged in',
      };
    });

    describe('start validation checking', () => {
      it('will have a startValidationChecking function', () =>
        expect(actions.startValidationChecking).toBeInstanceOf(Function));

      it(
        'will not call commit for the START_VALIDATION_CHECKING there is a validationInterval',
        () => {
          mutation.state.validationInterval = 1;
          actions.startValidationChecking(mutation);
          expect(mutation.commit.mock.calls[0]).toBeUndefined();
        },
      );

      it(
        'will not call commit for the START_VALIDATION_CHECKING when not running on the client',
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

      it('will return false', () => {
        const result = app.validate(store);
        expect(result).toEqual(false);
      });
    });
  });
});
