import mutations from '@/store/modules/cookieBanner/mutations';
import { SYNC, ACKNOWLEDGE } from '@/store/modules/cookieBanner/mutation-types';
import { mockCookies } from '../../../helpers';


describe('cookieBanner/mutiations.spec', () => {
  const SECONDS_IN_A_DAY = 60 * 60 * 24;

  beforeEach(() => {
    mutations.app = {
      $cookies: mockCookies(),
      $env: {
        SECURE_COOKIES: true,
        COOKIES_BANNER_EXPIRY_DAYS: 2,
      },
    };
  });

  describe('ACKNOWLEDGE', () => {
    it('will set the nhso.cookie_options cookie that indicating that the cookie terms have been reviewed', () => {
      const state = {};
      jest.spyOn(mutations.app.$cookies, 'set');
      mutations[ACKNOWLEDGE](state);

      expect(state.acknowledged).toEqual(true);
      expect(mutations.app.$cookies.set).toHaveBeenCalledWith(
        'nhso.cookie_options',
        true,
        { secure: true, maxAge: 2 * SECONDS_IN_A_DAY, path: '/' },
      );
    });
  });

  describe('SYNC', () => {
    describe('will restore nhso.cookie_options cookie or use that of the store ensuring that the cookie is not persisted again.', () => {
      it('when cookie is set.', () => {
        const state = {};
        jest.spyOn(mutations.app.$cookies, 'set');
        mutations.app.$cookies.get = jest.fn(() => true);
        mutations[SYNC](state);

        expect(state.acknowledged).toEqual(true);
        expect(mutations.app.$cookies.set).not.toHaveBeenCalledWith(
          'nhso.cookie_options',
          true,
          { secure: true, maxAge: 2 * SECONDS_IN_A_DAY, path: '/' },
        );
      });

      it('when cookie is not set.', () => {
        const state = { acknowledged: false };
        jest.spyOn(mutations.app.$cookies, 'set');
        mutations.app.$cookies.get = jest.fn(() => false);
        mutations[SYNC](state);

        expect(state.acknowledged).toEqual(false);
        expect(mutations.app.$cookies.remove).toHaveBeenCalledWith('nhso.cookie_options');
      });

      it('when cookie is set from the store.', () => {
        const state = { acknowledged: true };
        jest.spyOn(mutations.app.$cookies, 'set');
        mutations.app.$cookies.get = jest.fn(() => false);
        mutations[SYNC](state);

        expect(state.acknowledged).toEqual(true);
        expect(mutations.app.$cookies.set).toHaveBeenCalledWith(
          'nhso.cookie_options',
          true,
          { secure: true, maxAge: 2 * SECONDS_IN_A_DAY, path: '/' },
        );
      });
    });
  });
});
