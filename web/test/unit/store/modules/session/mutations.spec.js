import mutations from '@/store/modules/session/mutations';
import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  HIDE_SESSION_EXPIRING,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  SHOW_SESSION_EXPIRING,
  START_VALIDATION_CHECKING,
} from '@/store/modules/session/mutation-types';

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

  describe('SHOW_EXPIRY_MESSAGE', () => {
    it('will set the showExpiryMessage on the state to true', () => {
      const stateToMutate = {};
      mutations[SHOW_EXPIRY_MESSAGE](stateToMutate);
      expect(stateToMutate.showExpiryMessage).toBe(true);
    });
  });

  describe('HIDE_EXPIRY_MESSAGE', () => {
    it('will remove the showExpiryMessage property from the state', () => {
      const stateToMutate = {
        showExpiryMessage: true,
      };

      mutations[HIDE_EXPIRY_MESSAGE](stateToMutate);
      expect(stateToMutate.showExpiryMessage).toBeUndefined();
      expect(stateToMutate).not.toHaveProperty('showExpiryMessage');
    });
  });

  describe('START_VALIDATION_CHECKING', () => {
    it('will set the validationInterval property on the state', () => {
      const stateToMutate = {};

      mutations[START_VALIDATION_CHECKING](stateToMutate, 4);
      expect(stateToMutate.validationInterval).toEqual(4);
    });
  });

  describe('END_VALIDATION_CHECKING', () => {
    it('will clear the validationInterval property on the state', () => {
      const stateToMutate = {
        validationInterval: 5,
      };

      mutations[END_VALIDATION_CHECKING](stateToMutate);
      expect(stateToMutate.validationInterval).toBeUndefined();
    });
  });

  describe('SHOW_SESSION_EXPIRING', () => {
    it('will set the showSessionExpiring on the state to true', () => {
      const stateToMutate = {};
      mutations[SHOW_SESSION_EXPIRING](stateToMutate);
      expect(stateToMutate.showSessionExpiring).toBe(true);
    });
  });

  describe('HIDE_SESSION_EXPIRING', () => {
    it('will remove the showSessionExpiring property from the state', () => {
      const stateToMutate = {
        showSessionExpiring: true,
      };

      mutations[HIDE_SESSION_EXPIRING](stateToMutate);
      expect(stateToMutate.showSessionExpiring).toBeUndefined();
      expect(stateToMutate).not.toHaveProperty('showSessionExpiring');
    });
  });
});
