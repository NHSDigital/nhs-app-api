import { APPOINTMENTS, LOGIN, BEGINLOGIN, isAnonymous } from '@/lib/routes';

describe('routes', () => {
  describe('isAnonymous', () => {
    it('will be true for an anonymous route', () => {
      expect(isAnonymous(LOGIN)).toBe(true);
    });

    it('will be true for an anonymous route name', () => {
      expect(isAnonymous(LOGIN.name)).toBe(true);
    });

    it('will be true for an anonymous route', () => {
      expect(isAnonymous(BEGINLOGIN)).toBe(true);
    });

    it('will be true for an anonymous route name', () => {
      expect(isAnonymous(BEGINLOGIN.name)).toBe(true);
    });

    it('will be false for a non-anonymous route', () => {
      expect(isAnonymous(APPOINTMENTS)).toBe(false);
    });

    it('will be false for a non-anonymous route name', () => {
      expect(isAnonymous(APPOINTMENTS.name)).toBe(false);
    });
  });
});
