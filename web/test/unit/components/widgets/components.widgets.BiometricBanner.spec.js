import BiometricBanner from '@/components/widgets/BiometricBanner';
import { LOGIN_SETTINGS_PATH } from '@/router/paths';
import NativeCallbacks from '@/services/native-app';
import { redirectTo } from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/services/native-app');
jest.mock('@/lib/utils');

describe('BiometricBanner', () => {
  let wrapper;
  let $store;

  const mountBiometricBanner = ({
    isNativeApp = true,
    source = 'ios',
    dismissed = false,
    versionEnabled = true } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp,
          source,
        },
        biometricBanner: {
          dismissed,
        },
        loginSettings: {
          biometricType: undefined,
        },
      },
      getters: {
        'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(versionEnabled),
      },
    });
    return mount(BiometricBanner, {
      methods: {
        configureWebContext(url) {
          return url;
        },
      },
      $router: createRouter(),
      $route: { meta: { helpUrl: 'current-help-url' } },
      $store,
      $style: {
        info: 'info',
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('is native', () => {
    beforeEach(() => {
      NativeCallbacks.fetchBiometricSpec.mockClear();
    });
    describe('banner is dismissed', () => {
      beforeEach(() => {
        wrapper = mountBiometricBanner({ dismissed: true });
      });
      it('will not be visible if dismissed', () => {
        expect(wrapper.find('div').exists()).toBe(false);
      });
    });

    describe('banner is not dismissed', () => {
      beforeEach(() => {
        wrapper = mountBiometricBanner({ dismissed: false });
      });

      it('will be visible', () => {
        expect(wrapper.find('div').exists()).toBe(true);
      });
    });

    describe('dismiss button', () => {
      let dismissButton;

      beforeEach(() => {
        wrapper = mountBiometricBanner();
        dismissButton = wrapper.find('#btn_biometricBannerDismiss');
      });

      it('will be visible', () => {
        expect(dismissButton.exists()).toBe(true);
      });

      it('will dispatch `biometricBanner/dismiss` when clicked', () => {
        global.digitalData = {};
        $store.$cookies.set = jest.fn();
        dismissButton.trigger('click');
        expect($store.dispatch).toBeCalledWith('biometricBanner/dismiss');
        expect($store.dispatch).toBeCalledWith('biometricBanner/sync');
      });
    });

    describe('Biometric Link', () => {
      describe('biometrics web enabled', () => {
        describe('min app version met', () => {
          let biometricsLink;

          beforeEach(() => {
            wrapper = mountBiometricBanner({ versionEnabled: true });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
          });
        });

        describe('min app version not met', () => {
          let biometricsLink;
          beforeEach(() => {
            wrapper = mountBiometricBanner({ versionEnabled: false });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect(redirectTo).not.toHaveBeenCalled();
          });
        });
      });

      describe('biometrics web disabled', () => {
        describe('android', () => {
          let biometricsLink;

          beforeEach(() => {
            wrapper = mountBiometricBanner({ source: 'android' });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
          });
        });

        describe('ios', () => {
          let biometricsLink;

          beforeEach(() => {
            wrapper = mountBiometricBanner();
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
          });
        });
      });
    });
  });

  describe('is not native', () => {
    describe('is not native', () => {
      beforeEach(() => {
        wrapper = mountBiometricBanner({ isNativeApp: false, dismissed: true });
      });

      it('will not be visible if dismissed', () => {
        expect(wrapper.find('div').exists()).toBe(false);
      });
    });
    describe('is not native', () => {
      beforeEach(() => {
        wrapper = mountBiometricBanner({ isNativeApp: false, dismissed: false });
      });
      it('will not be visible if not dismissed', () => {
        expect(wrapper.find('div').exists()).toBe(false);
      });
    });
  });
});
