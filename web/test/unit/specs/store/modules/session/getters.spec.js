import moment from 'moment';
import getters from '@/store/modules/session/getters';

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
});
