import loginSettingsError from '@/pages/account/login-settings/error';
import { LOGIN_SETTINGS_PATH } from '@/router/paths';
import each from 'jest-each';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import biometricTypes from '@/lib/biometrics/biometricTypes';
import { redirectTo } from '@/lib/utils';
import { mount, createRouter, createStore } from '../../../helpers';

jest.mock('@/lib/utils');

describe('login settings page', () => {
  let wrapper;
  let store;
  let $router;

  const mountPage = ({
    biometricsRegistrationStatus = false,
    biometricType = 'loginSettings.biometrics.biometricType.face',
    biometricLocaleReference = biometricTypes.FaceID,
    isNativeApp = true,
    errorCode = undefined,
  } = {}) => {
    $router = createRouter();
    store = createStore({
      state: {
        loginSettings: {
          biometricsRegistrationStatus,
          biometricType,
          biometricLocaleReference,
          errorCode,
        },
        device: {
          isNativeApp,
        },
      },
      getters: {
        'loginSettings/retrieveError': errorCode,
        'loginSettings/getDeviceBiometricNameString': biometricType,
      },
    });
    wrapper = mount(loginSettingsError,
      { $store: store,
        data: () => ({
          cannotFindErrorText: `loginSettings.biometrics.errors.cannotFindBiometricType.errorText.${biometricLocaleReference}`,
        }),
        $router });
  };

  describe('biometric error screen', () => {
    describe('cannot find biometrics', () => {
      each([
        ['have information showing Face ID could not be found', 'face', 'Check that you have added a face scan in your device\'s Face ID settings.'],
        ['have information showing Touch ID could not be found', 'touch', 'Check that you have added a fingerprint in your device\'s Touch ID settings.'],
        ['have information showing Fingerprint could not be found', 'fingerPrint', 'Check that you have added a fingerprint in your device\'s security settings.'],
      ]).it('will %s', (_, biometricType, text) => {
        mountPage(
          {
            errorCode: biometricErrorCodes.CannotFindBiometrics,
            biometricType: `loginSettings.biometrics.biometricType.${biometricType}`,
            biometricLocaleReference: biometricType,
          },
        );
        expect(wrapper.findAll('p').at(0).text()).toContain(text);
      });
    });

    describe('cannot change biometric registration', () => {
      beforeEach(() => {
        mountPage({ errorCode: biometricErrorCodes.CannotChangeBiometrics });
      });

      it('will have information showing biometric registration could not be changed', () => {
        const paragraphs = wrapper.findAll('p');
        expect(paragraphs.at(0).text()).toContain('Go back and try again.');
        expect(paragraphs.at(1).text()).toContain('If you keep seeing this message, return to your settings later.');
      });
    });

    describe('undefined errorCode', () => {
      it('will redirect to login settings', async () => {
        mountPage();
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
      });
    });
  });
});
