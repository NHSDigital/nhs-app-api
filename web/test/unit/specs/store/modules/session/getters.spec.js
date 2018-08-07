import moment from 'moment';
import getters from '@/store/modules/session/getters';

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
