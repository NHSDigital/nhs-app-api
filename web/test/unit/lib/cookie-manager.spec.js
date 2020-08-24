import { setCookie, mergeCookie, removeCookies } from '@/lib/cookie-manager';
import { mockCookies } from '../helpers';


describe('cookie-manager', () => {
  let store;

  beforeEach(() => {
    store = {
      $cookies: mockCookies(),
    };
  });

  describe('setCookie', () => {
    describe.each([
      [true, true],
      [false, false],
      ['true', true],
      ['false', false],
      ['anything else', false],
    ])('will set a cookie', (secure, isSecure) => {
      it(`with ${secure} secure flag`, () => {
        setCookie({
          cookies: store.$cookies,
          key: 'nhso.terms',
          value: { areAccepted: true },
          secure,
        });

        expect(store.$cookies.set).toHaveBeenCalledWith(
          'nhso.terms',
          { areAccepted: true },
          null,
          '/',
          null,
          isSecure,
          'Lax',
        );
      });
    });

    it('will remove a cookie when blank', () => {
      setCookie({
        cookies: store.$cookies,
        key: 'nhso.terms',
        value: '',
        options: { secure: true },
      });

      expect(store.$cookies.set).not.toHaveBeenCalled();
    });

    it('will remove a cookie when undefined', () => {
      setCookie({
        cookies: store.$cookies,
        key: 'nhso.terms',
        value: '',
        options: { secure: true },
      });

      expect(store.$cookies.set).not.toHaveBeenCalled();
    });
  });

  describe('mergeCookie', () => {
    describe.each([
      [true, true],
      [false, false],
      ['true', true],
      ['false', false],
      ['anything else', false],
    ])('will create a new cookie', (secure, isSecure) => {
      it(`with ${secure} secure flag`, () => {
        store.$cookies.get = jest.fn(() => (undefined));

        mergeCookie({
          cookies: store.$cookies,
          key: 'nhso.terms',
          value: { areAccepted: true },
          secure,
        });

        expect(store.$cookies.set).toHaveBeenCalledWith(
          'nhso.terms',
          { areAccepted: true },
          null,
          '/',
          null,
          isSecure,
          'Lax',
        );
      });
    });

    it('will amalgamate an existing cookie', () => {
      store.$cookies.get = jest.fn(() => ({ areAccepted: false, updatedConsentRequired: true }));

      mergeCookie({
        cookies: store.$cookies,
        key: 'nhso.terms',
        value: { areAccepted: true },
        secure: true,
      });

      expect(store.$cookies.set).toHaveBeenCalledWith(
        'nhso.terms',
        { areAccepted: true, updatedConsentRequired: true },
        null,
        '/',
        null,
        true,
        'Lax',
      );
    });
  });

  describe('removeCookies', () => {
    it('will remove provided cookie', () => {
      removeCookies({
        cookies: store.$cookies,
        key: 'nhso.terms',
      });
      expect(store.$cookies.remove).toHaveBeenCalledTimes(1);
      expect(store.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });

    it('will remove provided cookies', () => {
      removeCookies({
        cookies: store.$cookies,
        key: ['nhso.terms', 'nhso.session'],
      });
      expect(store.$cookies.remove).toHaveBeenCalledTimes(2);
      expect(store.$cookies.remove.mock.calls[0]).toEqual(['nhso.terms']);
      expect(store.$cookies.remove.mock.calls[1]).toEqual(['nhso.session']);
    });
  });
});
