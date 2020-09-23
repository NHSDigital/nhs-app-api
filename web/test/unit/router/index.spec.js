/* eslint-disable import/no-duplicates */
import router, { isAnonymous } from '@/router';

import { resetPageFocus } from '@/lib/utils';
import store from '@/store';

jest.mock('@/lib/utils');
jest.mock('@/services/analytics-service');
jest.mock('@/store');

describe('isAnonymous', () => {
  it('will be true for an anonymous route', () => {
    const route = {
      path: '/login',
      name: 'Login',
      meta: {
        crumb: {},
        isAnonymous: true,
      },
    };
    expect(isAnonymous(route)).toBe(true);
  });

  it('will be false for a route with isAnonymous in meta false', () => {
    const route = {
      path: '/login',
      name: 'Login',
      meta: {
        crumb: {},
        isAnonymous: false,
      },
    };
    expect(isAnonymous(route)).toBe(false);
  });
});

describe('router', () => {
  describe('afterEach hooks', () => {
    it('will call resetPageFocus', () => {
      router.afterHooks.forEach(hook => hook({}));

      expect(resetPageFocus).toHaveBeenCalledWith(store);
    });
  });

  describe('patientId url parameter', () => {
    beforeEach(() => {
      store.state = {
        auth: {
          config: {
            some: 'value',
          },
        },
        session: {
          user: {
            some: 'value',
          },
        },
        serviceJourneyRules: {
          isLoaded: true,
        },
        linkedAccounts: {
          config: {
            hasLoaded: true,
          },
        },
        termsAndConditions: {
          areAccepted: true,
          updatedConsentRequired: false,
        },
        knownServices: {
          isLoaded: true,
        },
      };
      store.getters = {
        'session/isLoggedIn': () => true,
        'session/shouldUplift': () => true,
      };
    });
    describe.each([
      ['no guid', '/'],
      ['all letters', '/aPath/'],
      ['all numbers', '/12354/'],
    ])('for %s in url', (_, value) => {
      it('it will not populate patientId param', async () => {
        await router.push({ path: `/patient${value}appointments/gp-appointments` });
        expect(router.currentRoute.params.patientId).toBeUndefined();
      });
    });

    it('it will populate if guid exists in url', async () => {
      const GUID = '81e6fdab-ffb0-4cbe-8135-9775bbbe063f';
      await router.push({ path: `/patient/${GUID}/appointments/hospital-appointments` });
      expect(router.currentRoute.params.patientId).toBe(GUID);
    });
  });
});
