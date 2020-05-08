import loginSettingsError from '@/pages/account/login-settings/error';
import { mount, createRouter, createStore } from '../../../helpers';
import { LOGIN_SETTINGS } from '@/lib/routes';
import each from 'jest-each';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import biometricTypes from '@/lib/biometrics/biometricTypes';

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
        ['have information showing Face ID could not be found', 'face'],
        ['have information showing Touch ID could not be found', 'touch'],
        ['have information showing Fingerprint could not be found', 'fingerprint'],
      ]).it('will %s', (_, biometricType) => {
        mountPage(
          {
            errorCode: biometricErrorCodes.CannotFindBiometrics,
            biometricType: `loginSettings.biometrics.biometricType.${biometricType}`,
            biometricLocaleReference: biometricType,
          },
        );
        expect(wrapper.findAll('p').at(0).text())
          .toContain(`translate_loginSettings.biometrics.errors.cannotFindBiometricType.errorText.${biometricType}`);
      });
    });

    describe('cannot change biometric registration', () => {
      beforeEach(() => {
        mountPage({ errorCode: biometricErrorCodes.CannotChangeBiometrics });
      });

      it('will have information showing biometric registration could not be changed', () => {
        const paragraphs = wrapper.findAll('p');
        expect(paragraphs.at(0).text())
          .toContain('translate_loginSettings.biometrics.errors.cannotChangeBiometricSettings.paragraph1');
        expect(paragraphs.at(1).text())
          .toContain('translate_loginSettings.biometrics.errors.cannotChangeBiometricSettings.paragraph2');
      });
    });

    describe('undefined errorCode', () => {
      let redirect;

      beforeEach(() => {
        redirect = jest.fn();
      });

      it('will redirect to login settings', async () => {
        mountPage();
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).toBeCalledWith(LOGIN_SETTINGS.path);
      });
    });
  });
});
