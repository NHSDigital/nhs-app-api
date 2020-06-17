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
});
