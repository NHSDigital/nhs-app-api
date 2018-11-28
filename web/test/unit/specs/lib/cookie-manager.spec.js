import { setCookie, mergeCookie, removeCookies } from '@/lib/cookie-manager';
import { mockCookies } from '../../helpers';


describe('cookie-manager', () => {
  let app;

  beforeEach(() => {
    app = {
      $cookies: mockCookies(),
    };
  });

  describe('setCookie', () => {
    it('will set a cookie', () => {
      setCookie({
        cookies: app.$cookies,
        key: 'nhso.terms',
        value: { areAccepted: true },
        options: { secure: true },
      });

      expect(app.$cookies.set).toHaveBeenCalledWith(
        'nhso.terms',
        { areAccepted: true },
        { secure: true, path: '/' },
      );
    });


    it('will remove a cookie when blank', () => {
      setCookie({
        cookies: app.$cookies,
        key: 'nhso.terms',
        value: '',
        options: { secure: true },
      });

      expect(app.$cookies.set).not.toHaveBeenCalled();
    });

    it('will remove a cookie when undefined', () => {
      setCookie({
        cookies: app.$cookies,
        key: 'nhso.terms',
        value: '',
        options: { secure: true },
      });

      expect(app.$cookies.set).not.toHaveBeenCalled();
    });
  });

  describe('mergeCookie', () => {
    it('will create a new cookie', () => {
      app.$cookies.get = jest.fn(() => (undefined));

      mergeCookie({
        cookies: app.$cookies,
        key: 'nhso.terms',
        value: { areAccepted: true },
        options: { secure: true },
      });

      expect(app.$cookies.set).toHaveBeenCalledWith(
        'nhso.terms',
        { areAccepted: true },
        { secure: true, path: '/' },
      );
    });

    it('will amalgamate an existing cookie', () => {
      app.$cookies.get = jest.fn(() => ({ areAccepted: false, updatedConsentRequired: true }));

      mergeCookie({
        cookies: app.$cookies,
        key: 'nhso.terms',
        value: { areAccepted: true },
        options: { secure: true },
      });

      expect(app.$cookies.set).toHaveBeenCalledWith(
        'nhso.terms',
        { areAccepted: true, updatedConsentRequired: true },
        { secure: true, path: '/' },
      );
    });
  });

  describe('removeCookies', () => {
    it('will remove provided cookie', () => {
      removeCookies({
        cookies: app.$cookies,
        key: 'nhso.terms',
      });
      expect(app.$cookies.remove).toHaveBeenCalledTimes(1);
      expect(app.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });

    it('will remove provided cookies', () => {
      removeCookies({
        cookies: app.$cookies,
        key: ['nhso.terms', 'nhso.session'],
      });
      expect(app.$cookies.remove).toHaveBeenCalledTimes(2);
      expect(app.$cookies.remove.mock.calls[0]).toEqual(['nhso.terms']);
      expect(app.$cookies.remove.mock.calls[1]).toEqual(['nhso.session']);
    });
  });
});
