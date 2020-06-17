import CookieBanner from '@/components/CookieBanner';
import { createRouter, mount, createStore } from '../helpers';

describe('components/CookieBanner.vue -', () => {
  let $router;

  const createCookieBanner = ($route = {}) => {
    $router = createRouter();

    const $store = createStore({
      $router,
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
    return mount(CookieBanner, {
      $store,
      $router,
      $route,
    });
  };

  describe('created ', () => {
    it('will add push query to router when close button clicked', async () => {
      const cookieBanner = createCookieBanner();
      expect(cookieBanner.vm.showCookieBanner).toBe(true);

      const button = cookieBanner.find('#btn_closeCookieBanner');
      button.trigger('click');

      expect($router.push).toHaveBeenCalledWith({ query: { acknowledged: 'true' } });
    });

    it('will show banner if query param not present', async () => {
      const cookieBanner = createCookieBanner();
      expect(cookieBanner.vm.showCookieBanner).toBe(true);
    });

    it('will not show banner if query param present', async () => {
      const $route = {
        query: { acknowledged: 'true' },
      };
      const cookieBanner = createCookieBanner($route);
      expect(cookieBanner.vm.showCookieBanner).toBe(false);
    });
  });
});
