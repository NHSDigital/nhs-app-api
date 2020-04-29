import mockdate from 'mockdate';
import moment from 'moment';
import mutations from '@/store/modules/preRegistrationInformation/mutations';
import { CONTINUE, SYNC } from '@/store/modules/preRegistrationInformation/mutation-types';
import { mockCookies } from '../../../helpers';

describe('pre-registration information mutations', () => {
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

  describe('CONTINUE', () => {
    let state;

    beforeEach(() => {
      state = {};
      mutations[CONTINUE](state);
    });

    it('will set `dismissed` state to true', () => {
      expect(state.seen).toEqual(true);
    });

    it('will set cookie to true', () => {
      expect(mutations.app.$cookies.set).toHaveBeenCalledWith(
        'SkipPreRegistrationPage',
        true,
        { secure: true, maxAge: expirySeconds, path: '/' },
      );
    });
  });

  describe('SYNC', () => {
    let state;
    let skipPreRegistrationPageValue;

    beforeEach(() => {
      state = {};
      mutations.app.$cookies.get = jest.fn().mockImplementation((name) => {
        switch (name) {
          case 'SkipPreRegistrationPage':
            return skipPreRegistrationPageValue;
          default:
            return undefined;
        }
      });
    });

    describe('cookie set to true', () => {
      beforeEach(() => {
        skipPreRegistrationPageValue = true;
        mutations[SYNC](state);
      });

      it('will set `seen` state to true', () => {
        expect(state.seen).toBe(true);
      });

      it('will not set cookie', () => {
        expect(mutations.app.$cookies.set).not.toBeCalled();
      });
    });

    describe('cookie not set', () => {
      beforeEach(() => {
        skipPreRegistrationPageValue = undefined;
      });

      describe('not seen', () => {
        beforeEach(() => {
          state.dismissed = false;
          mutations[SYNC](state);
        });

        it('will set `dismissed` state to false', () => {
          expect(state.seen).toBe(false);
        });

        it('will not set cookie', () => {
          expect(mutations.app.$cookies.set).not.toBeCalled();
        });
      });

      describe('seen', () => {
        beforeEach(() => {
          state.seen = true;
          mutations[SYNC](state);
        });

        it('will set `seen` state to true', () => {
          expect(state.seen).toBe(true);
        });

        it('will set cookie to true', () => {
          expect(mutations.app.$cookies.set).toBeCalledWith(
            'SkipPreRegistrationPage',
            true,
            { secure: true, maxAge: expirySeconds, path: '/' },
          );
        });
      });
    });
  });
});
