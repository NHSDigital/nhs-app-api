import BiometricBanner from '@/components/widgets/BiometricBanner';
import { LOGIN_SETTINGS } from '@/lib/routes';
import { createRouter, createStore, mount } from '../../helpers';
import NativeCallbacks from '@/services/native-app';

jest.mock('@/services/native-app');

describe('BiometricBanner', () => {
  let $router;
  let wrapper;
  let $store;

  const mountBiometricBanner = ({
    webBiometrics = false,
    isNativeApp = true,
    source = 'ios',
    dismissed = false,
    versionEnabled = true } = {}) => {
    $router = createRouter();
    $store = createStore({
      $env: {
        WEB_BIOMETRICS_ENABLED: webBiometrics,
      },
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
      $router,
      methods: {
        configureWebContext(url) {
          return url;
        },
      },
      $store,
      $style: {
        info: 'info',
      },
    });
  };

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
        dismissButton.trigger('click');
        expect($store.dispatch).toBeCalledWith('biometricBanner/dismiss');
      });
    });

    describe('Biometric Link', () => {
      describe('biometrics web enabled', () => {
        describe('min app version met', () => {
          let biometricsLink;

          beforeEach(() => {
            wrapper = mountBiometricBanner({ webBiometrics: true, versionEnabled: true });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect($router.push).toHaveBeenCalledWith(LOGIN_SETTINGS.path);
          });
        });

        describe('min app version not met', () => {
          let biometricsLink;
          beforeEach(() => {
            wrapper = mountBiometricBanner({ webBiometrics: true, versionEnabled: false });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect($router.push).not.toHaveBeenCalledWith(LOGIN_SETTINGS.path);
          });
        });
      });

      describe('biometrics web disabled', () => {
        describe('android', () => {
          let biometricsLink;

          beforeEach(() => {
            wrapper = mountBiometricBanner({ webBiometrics: false, source: 'android' });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will not navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect($router.push).not.toHaveBeenCalledWith(LOGIN_SETTINGS.path);
          });
        });

        describe('ios', () => {
          let biometricsLink;

          beforeEach(() => {
            wrapper = mountBiometricBanner({ webBiometrics: false });
            biometricsLink = wrapper.find('#btn_goToSettings');
          });

          it('will navigate to the web biometrics', () => {
            biometricsLink.trigger('click');
            expect($router.push).toHaveBeenCalledWith(LOGIN_SETTINGS.path);
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
