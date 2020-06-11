import consola from 'consola';
import each from 'jest-each';
import {
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENTS,
  BEGINLOGIN,
  HEALTH_RECORDS,
  INDEX,
  LOGIN,
  MESSAGES,
  PRESCRIPTIONS,
  SYMPTOMS,
  TERMSANDCONDITIONS,
  backLinkOverrides,
  executeHomeNavigationRule,
  findByName,
  findByPage,
  findByPath,
  getRouteNames,
  isAnonymous,
} from '@/lib/routes';
import { AppPage } from '@/static/js/v1/src/constants';

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

  describe('backLinkOverrides', () => {
    each([
      ['account-cookies', '/account', true],
      ['appointments-gp-appointments-booking-success', '/appointments', true],
      ['appointments-gp-appointments-cancelling-success', '/appointments', true],
      ['linked-profiles-shutter-appointments', '/appointments', true],
      ['linked-profiles-shutter-prescriptions', '/', true],
      ['organ-donation', '/more', undefined],
      ['organ-donation-view-decision', '/more', undefined],
      ['messages-gp-messages', '/messages', true],
      ['switch-profile', '/', true]])
      .it('will go to $path from $name by default', (name, path, ignoreStore) => {
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

  describe('getRouteByName', () => {
    it('should not return route object for an unnamed route.', () => {
      expect(findByName('unnamed')).toBeUndefined();
    });

    it('should return the corresponding route object for a given name.', () => {
      expect(findByName('appointments-gp-appointments-booking-guidance')).toEqual(APPOINTMENT_BOOKING_GUIDANCE);
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

  describe('findByPage', () => {
    let result;
    let errorFn;

    beforeEach(() => {
      errorFn = consola.error;
      consola.error = jest.fn();
    });

    describe('with unknown page', () => {
      beforeEach(() => {
        result = findByPage('foo');
      });

      it('will return `undefined`', () => {
        expect(result).toBeUndefined();
      });

      it('will log an error', () => {
        expect(consola.error).toBeCalled();
      });
    });

    describe.each([
      [AppPage.HOME_PAGE, INDEX],
      [AppPage.APPOINTMENTS, APPOINTMENTS],
      [AppPage.PRESCRIPTIONS, PRESCRIPTIONS],
      [AppPage.HEALTH_RECORDS, HEALTH_RECORDS],
      [AppPage.SYMPTOMS, SYMPTOMS],
      [AppPage.MESSAGES, MESSAGES],
    ])('with `%s` page', (page, route) => {
      beforeEach(() => {
        result = findByPage(page);
      });

      it(`will return the ${route.name} route`, () => {
        expect(result).toBe(route);
      });

      it('will not log an error', () => {
        expect(consola.error).not.toBeCalled();
      });
    });

    afterEach(() => {
      consola.error = errorFn;
    });
  });

  describe('getHelpUrlByRouteName', () => {
    it('should return the expected helpUrl for the route name', () => {
      const appointmentsHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/appointments/';
      const prescriptionsHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/prescriptions/';
      const recordHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/record/';
      const onlineConsultationsHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/online-consultations/';
      const messagingHelpUrl = 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/messaging/';

      expect(findByName('appointments').helpUrl).toBe(appointmentsHelpUrl);
      expect(findByName('prescriptions').helpUrl).toBe(prescriptionsHelpUrl);
      expect(findByName('my-record').helpUrl).toBe(recordHelpUrl);
      expect(findByName('appointments-gp-appointments-admin-help').helpUrl).toBe(onlineConsultationsHelpUrl);
      expect(findByName('appointments-gp-appointments-gp-advice').helpUrl).toBe(onlineConsultationsHelpUrl);
      expect(findByName('my-record').helpUrl).toBe(recordHelpUrl);
      expect(findByName('messages').helpUrl).toBe(messagingHelpUrl);
      expect(findByName('messages-app-messaging').helpUrl).toBe(messagingHelpUrl);
      expect(findByName('messages-app-messaging-app-message').helpUrl).toBe(messagingHelpUrl);
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
});
