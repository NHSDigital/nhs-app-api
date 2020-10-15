import CookieBanner from '@/components/CookieBanner';
import { mount, createStore } from '../helpers';

describe('CookieBanner', () => {
  let $store;
  const createCookieBanner = ({ acknowledged = false, isNativeApp = false } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp,
        },
        cookieBanner: {
          acknowledged,
        },
      },
    });
    return mount(CookieBanner, { $store });
  };

  describe('methods', () => {
    it('will set acknowledged in the store and sessionStorage when close button clicked', () => {
      Storage.prototype.setItem = jest.fn();

      const cookieBanner = createCookieBanner();
      expect(cookieBanner.vm.showCookieBanner).toBe(true);

      const button = cookieBanner.find('#btn_closeCookieBanner');
      button.trigger('click');

      expect(sessionStorage.setItem).toHaveBeenCalled();
    });
  });

  describe('computed', () => {
    let cookieBanner;

    beforeEach(() => {
      cookieBanner = createCookieBanner();
    });
    it('will show banner if hasAcknowledgedCookies in sessionStorage is false', () => {
      Storage.prototype.getItem = jest.fn('hasAcknowledgedCookies').mockImplementation(() => false);
      expect(cookieBanner.vm.showCookieBanner).toBe(true);
    });

    it('will not show banner if hasAcknowledgedCookies is set in the session storage', () => {
      Storage.prototype.getItem = jest.fn('hasAcknowledgedCookies').mockImplementation(() => true);
      // This needs to be created after the sessionStorage is mocked
      cookieBanner = createCookieBanner();
      expect(cookieBanner.vm.showCookieBanner).toBe(false);
    });

    it('will not show banner if hasAcknowledgedCookies is set in the session storage and acknowledged is set', () => {
      Storage.prototype.getItem = jest.fn('hasAcknowledgedCookies').mockImplementation(() => true);
      // This needs to be created after the sessionStorage is mocked
      cookieBanner = createCookieBanner({ acknowledged: true });
      expect(cookieBanner.vm.showCookieBanner).toBe(false);
    });
    it('will not show banner if on a native device', () => {
      cookieBanner = createCookieBanner({ isNativeApp: true });
      expect(cookieBanner.vm.showCookieBanner).toBe(false);
    });
  });
});
