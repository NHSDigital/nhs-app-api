import Vuex from 'vuex';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import CookieBanner from '@/components/CookieBanner';

const $t = key => `translate_${key}`;
const createCookieBanner = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  return shallowMount(CookieBanner, {
    localVue,
    mocks: {
      $http,
      $store,
      $t,
      $style: {
        info: 'info',
      },
      showTemplate: () => true,
    },
    stubs: {
      'nuxt-link': '<a></a>',
    },
  });
};

const createStore = () => ({
  dispatch: jest.fn(() => Promise.resolve()),
  state: {
    cookieBanner: {
      acknowledged: false,
    },
    device: {
      isNativeApp: false,
    },
  },
  app: {
    $env: {
      COOKIES_BANNER_URL: 'infoAboutCookies',
    },
  },
});

describe('components/CookieBanner.vue -', () => {
  describe('created ', () => {
    it('will issue cookie/store synchronisation', async () => {
      const $store = createStore();

      jest.spyOn($store, 'dispatch');

      createCookieBanner($store);

      expect($store.dispatch).toHaveBeenCalledWith('cookieBanner/sync');
    });
  });

  describe('onCookieBannerClicked ', () => {
    it('will issue cookie acknowledgement event when in js mode', async () => {
      const $store = createStore();

      jest.spyOn($store, 'dispatch');

      const cookieBanner = createCookieBanner($store);
      cookieBanner.vm.onCookieBannerClicked();

      expect($store.dispatch).toHaveBeenCalledWith('cookieBanner/acknowledge');
    });
  });
});
