import moment from 'moment';
import getters from '@/store/modules/session/getters';
import proofLevel from '@/lib/proofLevel';

describe('getters', () => {
  describe('isLoggedIn', () => {
    const { isLoggedIn } = getters;

    describe('session state', () => {
      it('will be true if the csrfToken is set', () => {
        const currentState = { csrfToken: 'boo' };
        expect(isLoggedIn(currentState)()).toEqual(true);
      });

      it('will be false if the csrfToken is not set', () => {
        const currentState = { csrfToken: undefined };
        expect(isLoggedIn(currentState)()).toEqual(false);
      });
    });

    describe('global state', () => {
      it('will be true if the csrfToken is set', () => {
        const currentState = { session: { csrfToken: 'boo' } };
        expect(isLoggedIn(currentState)()).toEqual(true);
      });

      it('will be false if the csrfToken is not set', () => {
        const currentState = { session: { csrfToken: undefined } };
        expect(isLoggedIn(currentState)()).toEqual(false);
      });
    });
  });

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

  describe('isExpiring', () => {
    const { isExpiring } = getters;

    it('will be false if lastCalledAt is not set', () => {
      const currentState = { durationSeconds: 430 };
      const expiringWarningSeconds = 15;
      expect(isExpiring(currentState)(expiringWarningSeconds)).toEqual(false);
    });

    it('will be false if durationSeconds is not set', () => {
      const currentState = { lastCalledAt: new Date() };
      const expiringWarningSeconds = 15;
      expect(isExpiring(currentState)(expiringWarningSeconds)).toEqual(false);
    });

    it('will be false if expiringWarningSeconds is not set', () => {
      const currentState = { durationSeconds: 430, lastCalledAt: new Date() };
      expect(isExpiring(currentState)()).toEqual(false);
    });

    it('will be false if now is greater than lastCalledAt plus durationSeconds', () => {
      const now = moment();
      const durationSeconds = 5000;
      const expiringWarningSeconds = 100;
      const lastCalledAt = now.subtract(durationSeconds + 1, 'seconds');
      const currentState = { durationSeconds, lastCalledAt };

      expect(isExpiring(currentState)(expiringWarningSeconds)).toEqual(false);
    });

    it('will be true if now is less than lastCalledAt plus durationSeconds and within expiringWarningSeconds', () => {
      const now = moment();
      const durationSeconds = 5000;
      const expiringWarningSeconds = 100;
      const lastCalledAt = now.subtract((durationSeconds - expiringWarningSeconds) + 1, 'seconds');
      const currentState = { durationSeconds, lastCalledAt };

      expect(isExpiring(currentState)(expiringWarningSeconds)).toEqual(true);
    });

    it('will be false if now is less than lastCalledAt plus durationSeconds minus and outside expiringWarningSeconds', () => {
      const now = moment();
      const durationSeconds = 5000;
      const expiringWarningSeconds = 100;
      const lastCalledAt = now.subtract((durationSeconds - expiringWarningSeconds) - 50, 'seconds');
      const currentState = { durationSeconds, lastCalledAt };

      expect(isExpiring(currentState)(expiringWarningSeconds)).toEqual(false);
    });
  });

  describe('isProxying', () => {
    const { isProxying } = getters;
    it('returns true when acting as another user', () => {
      const currentState = {};
      const rootState = {
        linkedAccounts: {
          actingAsUser: {
            id: 'user-id-0',
          },
        },
      };

      // act / assert
      expect(isProxying(currentState, getters, rootState)).toBe(true);
    });

    it('returns false when not acting as another user', () => {
      const currentState = {};
      const rootState = {
        linkedAccounts: {
          actingAsUser: null,
        },
      };

      // act / assert
      expect(isProxying(currentState, getters, rootState)).toBe(false);
    });
  });

  describe('currentProfile', () => {
    const { currentProfile } = getters;
    it('returns proxy user details when acting as another user', () => {
      const proxyUserDetails = {
        ageMonths: '10',
        ageYears: '39',
        givenName: 'harry',
        fullName: 'harry dixon',
      };

      const mockGetters = {
        isProxying: true,
      };

      const currentSessionState = {};
      const rootState = {
        linkedAccounts: {
          actingAsUser: proxyUserDetails,
        },
      };

      // act / assert
      expect(currentProfile(currentSessionState, mockGetters, rootState))
        .toEqual(proxyUserDetails);
    });

    it('returns session user details when not acting as another user', () => {
      const sessionUserDetails = {
        nhsNumber: '987123456',
        name: 'harry',
        dateOfBirth: '2001-01-01',
      };

      const currentSessionState = {
        user: sessionUserDetails.name,
        nhsNumber: sessionUserDetails.nhsNumber,
        dateOfBirth: sessionUserDetails.dateOfBirth,
      };

      const mockGetters = {
        isProxying: false,
      };

      const rootState = {
        linkedAccounts: {
          actingAsUser: null,
        },
      };

      // act / assert
      expect(currentProfile(currentSessionState, mockGetters, rootState))
        .toEqual(sessionUserDetails);
    });
  });

  describe('shouldUplift', () => {
    const { shouldUplift } = getters;
    let state;

    describe('for P9 user', () => {
      let action;

      beforeEach(() => {
        state = { proofLevel: proofLevel.P9 };
        action = shouldUplift(state);
      });

      it('will be false when it requires proof level 5', () => {
        expect(action(proofLevel.P5)).toBe(false);
      });

      it('will be false when it requires proof level 9', () => {
        expect(action(proofLevel.P9)).toBe(false);
      });
    });

    describe('for P5 user', () => {
      let action;

      beforeEach(() => {
        state = { proofLevel: proofLevel.P5 };
        action = shouldUplift(state);
      });

      it('will be false when it requires proof level 5', () => {
        expect(action(proofLevel.P5)).toBe(false);
      });

      it('will be true when it requires proof level 9', () => {
        expect(action(proofLevel.P9)).toBe(true);
      });
    });
  });
});
