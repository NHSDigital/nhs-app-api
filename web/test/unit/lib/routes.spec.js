import { TERMSANDCONDITIONS, APPOINTMENTS, LOGIN, BEGINLOGIN, isAnonymous, executeHomeNavigationRule } from '@/lib/routes';

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

  describe('executeHomeNavigationRule', () => {
    it('terms and condition header link should resolve to logout', () => {
      expect(executeHomeNavigationRule(TERMSANDCONDITIONS.name)).toBe('/logout');
    });

    it('anything route\'s  header link should resolve to index', () => {
      expect(executeHomeNavigationRule(APPOINTMENTS.name)).toBe('/');
    });
  });
});
