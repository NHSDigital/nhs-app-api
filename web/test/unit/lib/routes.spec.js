import {
  TERMSANDCONDITIONS,
  INDEX,
  APPOINTMENTS,
  APPOINTMENT_BOOKING_GUIDANCE,
  LOGIN,
  BEGINLOGIN,

  isAnonymous,
  executeHomeNavigationRule,
  getCrumbTrailForRoute,
  getRouteNames,
  findByName,
} from '@/lib/routes';

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

  describe('getCrumbTrailForRoute', () => {
    it('should return the corresponding crumb trail with some depth', () => {
      expect(getCrumbTrailForRoute(APPOINTMENT_BOOKING_GUIDANCE)).toEqual([INDEX, APPOINTMENTS]);
    });

    it('should return the corresponding crumb trail with no depth', () => {
      expect(getCrumbTrailForRoute(INDEX)).toEqual([]);
    });
  });

  describe('getRouteByName', () => {
    it('should not return route object for an unnamed route.', () => {
      expect(findByName('unnamed')).toBeUndefined();
    });

    it('should return the corresponding route object for a given name.', () => {
      expect(findByName('appointments-booking-guidance')).toEqual(APPOINTMENT_BOOKING_GUIDANCE);
    });
  });

  describe('Each route name', () => {
    it('should be unique.', (done) => {
      const routeNames = getRouteNames();

      expect(routeNames.length).toBeTruthy();

      const foundMap = {};
      routeNames.forEach((name) => {
        if (foundMap[name]) {
          done.fail(`[${name}] is duplicated and should be unique.`);
        } else {
          foundMap[name] = true;
        }
      });
      done();
    });
  });


  describe('check the depth of the crumb trail does ' +
    'not breach 3 levels as per the spec - see NHSO-4085', () => {
    it('should ensure that all calculated crumb trail does not breach 3 levels.', () => {
      const routeNames = getRouteNames();

      expect(routeNames.length).toBeTruthy();

      routeNames
        .map(name => findByName(name))
        .forEach(route => expect(getCrumbTrailForRoute(route).length <= 3).toBe(true));
    });
  });
});
