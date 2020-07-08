import mockdate from 'mockdate';
import moment from 'moment';
import mutations from '@/store/modules/biometricBanner/mutations';
import { DISMISS, SYNC } from '@/store/modules/biometricBanner/mutation-types';
import { mockCookies } from '../../../helpers';

describe('biometric banner mutations', () => {
  let expirySeconds;

  beforeEach(() => {
    mutations.app = {
      $cookies: mockCookies(),
      $env: {
        SECURE_COOKIES: true,
        COOKIES_BANNER_EXPIRY_DAYS: 2,
      },
    };

    jest.spyOn(mutations.app.$cookies, 'set');

    const nowDate = moment.duration(5, 'y');
    expirySeconds = nowDate.asSeconds();
    mockdate.set(nowDate);
  });

  afterEach(() => {
    mockdate.reset();
  });

  describe('DISMISS', () => {
    let state;

    beforeEach(() => {
      state = {};
      mutations[DISMISS](state);
    });

    it('will set `dismissed` state to true', () => {
      expect(state.dismissed).toEqual(true);
    });
  });

  describe('SYNC', () => {
    let state;
    let hideBiometricBannerValue;

    beforeEach(() => {
      state = {};
      mutations.app.$cookies.get = jest.fn().mockImplementation((name) => {
        switch (name) {
          case 'HideBiometricBanner':
            return hideBiometricBannerValue;
          default:
            return undefined;
        }
      });
    });

    describe('cookie set to true', () => {
      beforeEach(() => {
        hideBiometricBannerValue = true;
        mutations[SYNC](state);
      });

      it('will set `dismissed` state to true', () => {
        expect(state.dismissed).toBe(true);
      });

      it('will not set cookie', () => {
        expect(mutations.app.$cookies.set).not.toBeCalled();
      });
    });

    describe('cookie not set', () => {
      beforeEach(() => {
        hideBiometricBannerValue = undefined;
      });

      describe('not dismissed', () => {
        beforeEach(() => {
          state.dismissed = false;
          mutations[SYNC](state);
        });

        it('will set `dismissed` state to false', () => {
          expect(state.dismissed).toBe(false);
        });

        it('will not set cookie', () => {
          expect(mutations.app.$cookies.set).not.toBeCalled();
        });
      });

      describe('dismissed', () => {
        beforeEach(() => {
          state.dismissed = true;
          mutations[SYNC](state);
        });

        it('will set `dismissed` state to true', () => {
          expect(state.dismissed).toBe(true);
        });

        it('will set cookie to true', () => {
          expect(mutations.app.$cookies.set).toBeCalledWith(
            'HideBiometricBanner',
            true,
            { secure: true, maxAge: expirySeconds, path: '/' },
          );
        });
      });
    });
  });
});
