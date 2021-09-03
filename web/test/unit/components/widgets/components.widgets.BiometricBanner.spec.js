import BiometricBanner from '@/components/widgets/BiometricBanner';
import NativeApp from '@/services/native-app';
import {
  MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH,
  MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import biometricTypes from '@/lib/biometrics/biometricTypes';
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
  } = {}) => {
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
          biometricSupported: false,
        },
      },
    });
    return mount(BiometricBanner, {
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
      NativeApp.fetchBiometricSpec.mockClear();
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

    describe.each([
      [true, biometricTypes.Fingerprint, 'Set up fingerprint', MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH],
      [true, biometricTypes.FingerprintFaceOrIris, 'Set up fingerprint, face or iris', MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH],
      [true, biometricTypes.TouchID, 'Set up Touch ID', MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH],
      [true, biometricTypes.FaceID, 'Set up Face ID', MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH],
      [true, undefined, 'Set up login options', MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH],
      [false, undefined, 'Open login settings', MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH],
    ])('Biometric button text', (biometricSupported, biometricType, expectedText, expectedRedirectPath) => {
      let biometricsLink;

      beforeEach(() => {
        wrapper = mountBiometricBanner({ dismissed: false });
        $store.getters = {
          'loginSettings/biometricSupported': biometricSupported,
          'loginSettings/biometricType': biometricType,
        };
        biometricsLink = wrapper.find('#btn_goToSettings');
      });

      it(`will contain the text '${expectedText}'`, () => {
        expect(biometricsLink.text()).toBe(expectedText);
      });

      it('will navigate to the biometrics page', () => {
        biometricsLink.trigger('click');
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm,
          expectedRedirectPath);
      });
    });

    describe.each([
      [true, biometricTypes.Fingerprint, 'Set up fingerprint'],
      [true, biometricTypes.TouchID, 'Set up Touch ID'],
      [true, biometricTypes.FaceID, 'Set up Face ID'],
      [true, biometricTypes.FingerprintFaceOrIris, 'Set up fingerprint, face or iris'],
      [true, undefined, 'Set up login options'],
      [false, undefined, 'Open login settings'],
    ])('Biometric button text', (biometricSupported, biometricType, expectedText) => {
      let biometricsLink;

      beforeEach(() => {
        wrapper = mountBiometricBanner({ dismissed: false });
        $store.getters = {
          'loginSettings/biometricSupported': biometricSupported,
          'loginSettings/biometricType': biometricType,
        };
        biometricsLink = wrapper.find('#btn_goToSettings');
      });

      it(`will contain the text '${expectedText}'`, () => {
        expect(biometricsLink.text()).toBe(expectedText);
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
