import Vuex from 'vuex';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import BiometricBanner from '@/components/widgets/BiometricBanner';
import { mockCookies } from '../../helpers';
import { setCookie } from '@/lib/cookie-manager';

const $t = key => `translate_${key}`;
const createBiometricBanner = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  return shallowMount(BiometricBanner, {
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
  });
};

const createStore = () => ({
  dispatch: jest.fn(() => Promise.resolve()),
  state: {
    device: {
      source: 'android',
    },
  },
  app: {
    $cookies: mockCookies(),
    $env: {
      SECURE_COOKIES: true,
    },
  },
});

describe('components/BiometricBanner.vue -', () => {
  describe('is visible ', () => {
    it('will display the banner when there is no cookie and it is a native app', async () => {
      const $store = createStore();

      jest.spyOn($store, 'dispatch');

      $store.app.$cookies.get = jest.fn(() => undefined);

      const biometricBanner = createBiometricBanner($store);

      expect(biometricBanner.find('[id="btn_biometricBannerDismiss"]').exists()).toBe(true);

      $store.state.device.source = 'ios';
      expect(biometricBanner.find('[id="btn_biometricBannerDismiss"]').exists()).toBe(true);
    });
  });

  describe('on Dismiss banner clicked ', () => {
    it('will issue cookie to dismiss banner when in JS mode', async () => {
      const $store = createStore();

      jest.spyOn($store, 'dispatch');

      const biometricBanner = createBiometricBanner($store);
      biometricBanner.vm.dismissBiometricsBannerClicked();
      setCookie({
        cookies: $store.app.$cookies,
        key: 'HideBiometricBanner',
        value: true,
        options: { secure: true },
      });

      expect($store.app.$cookies.set).toHaveBeenCalledWith(
        'HideBiometricBanner',
        true,
        { secure: true, path: '/' },
      );
    });
  });
  describe('is not visible ', () => {
    it('will not display the banner when there is a cookie and it is a native app', async () => {
      const $store = createStore();
      setCookie({
        cookies: $store.app.$cookies,
        key: 'HideBiometricBanner',
        value: true,
        options: { secure: true },
      });
      $store.state.device.source = 'web';

      const biometricBanner = createBiometricBanner($store);

      expect(biometricBanner.find('[id="btn_biometricBannerDismiss"]').exists()).toBe(false);
    });
  });
});
