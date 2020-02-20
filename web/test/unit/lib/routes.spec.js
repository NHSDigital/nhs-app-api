import {
  TERMSANDCONDITIONS,
  INDEX,
  APPOINTMENTS,
  APPOINTMENT_BOOKING_GUIDANCE,
  LOGIN,
  BEGINLOGIN,
  GP_APPOINTMENTS,
  backLinkOverrides,
  isAnonymous,
  executeHomeNavigationRule,
  getCrumbTrailForRoute,
  getRouteNames,
  findByName,
  findByPath,
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

  /* eslint-disable indent */
  describe('backLinkOverrides', () => {
    it.each`
      name                                          | path                              | ignoreStore
      ${'organ-donation'}                           | ${'/more'}                        | ${undefined}
      ${'organ-donation-view-decision'}             | ${'/more'}                        | ${undefined}
      ${'patient-practice-messaging-view-details'}  | ${'/patient-practice-messaging'}  | ${true}
      ${'switch-profile'}                           | ${'/'}                            | ${true}
      `('will go to $path from $name by default', ({ name, path, ignoreStore }) => {
      const override = backLinkOverrides[name];
      expect(override.defaultPath).toBe(path);
      expect(override.ignoreStore).toBe(ignoreStore);
    });

    it('will have a default path in each overriding configuration', () => {
      Object.keys(backLinkOverrides).forEach((key) => {
        expect(backLinkOverrides[key].defaultPath).toBeDefined();
      });
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
      expect(getCrumbTrailForRoute(APPOINTMENT_BOOKING_GUIDANCE))
        .toEqual([INDEX, APPOINTMENTS, GP_APPOINTMENTS]);
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

  describe('findByPath', () => {
    it('should not return route object for a not found path.', () => {
      expect(findByPath('invalid')).toBeUndefined();
    });

    it('should return the corresponding route object for a given path.', () => {
      expect(findByPath('/appointments')).toEqual(APPOINTMENTS);
    });
  });

  describe('getHelpUrlByRouteName', () => {
    it('should return the expected helpUrl for the route name', () => {
      const appointmentsHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/appointments/';
      const prescriptionsHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/prescriptions/';
      const recordHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/record/';
      const onlineConsultationsHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/online-consultations/';

      expect(findByName('appointments').helpUrl).toBe(appointmentsHelpUrl);
      expect(findByName('prescriptions').helpUrl).toBe(prescriptionsHelpUrl);
      expect(findByName('my-record').helpUrl).toBe(recordHelpUrl);
      expect(findByName('appointments-admin-help').helpUrl).toBe(onlineConsultationsHelpUrl);
      expect(findByName('appointments-gp-advice').helpUrl).toBe(onlineConsultationsHelpUrl);
    });
  });

  describe('checkEachRouteHasValidHelpUrlAttribute', () => {
    it('should have a valid helpUrl attribute', (done) => {
      const routeNames = getRouteNames();
      const expression = /https?:\/\/(www\.)?[-a-zA-Z0-9@:%._+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_+.~#?&//=]*)?/gi;
      const regex = new RegExp(expression);

      expect(routeNames.length).toBeTruthy();

      routeNames.map(name => findByName(name))
        .forEach((route) => {
          if (route.helpUrl === undefined ||
            route.helpUrl === null ||
            route.helpUrl === '' ||
            !route.helpUrl.match(regex)) {
            done.fail(`[${route.name}] needs a valid populated helpUrl attribute.`);
          }
        });
      done();
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
