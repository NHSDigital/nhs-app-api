import mutations from '@/store/modules/preRegistrationInformation/mutations';
import { CONTINUE, SYNC } from '@/store/modules/preRegistrationInformation/mutation-types';
import { mockCookies } from '../../../helpers';

describe('pre-registration information mutations', () => {
  const expires = '5y';

  beforeEach(() => {
    mutations.$cookies = mockCookies();
    mutations.$env = {
      SECURE_COOKIES: true,
    };

    jest.spyOn(mutations.$cookies, 'set');
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
      expect(mutations.$cookies.set).toHaveBeenCalledWith(
        'SkipPreRegistrationPage',
        true,
        expires,
        '/',
        null,
        true,
        'Lax',
      );
    });
  });

  describe('SYNC', () => {
    let state;
    let skipPreRegistrationPageValue;

    beforeEach(() => {
      state = {};
      mutations.$cookies.get = jest.fn().mockImplementation((name) => {
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
        expect(mutations.$cookies.set).not.toBeCalled();
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
          expect(mutations.$cookies.set).not.toBeCalled();
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
          expect(mutations.$cookies.set).toBeCalledWith(
            'SkipPreRegistrationPage',
            true,
            expires,
            '/',
            null,
            true,
            'Lax',
          );
        });
      });
    });
  });
});
