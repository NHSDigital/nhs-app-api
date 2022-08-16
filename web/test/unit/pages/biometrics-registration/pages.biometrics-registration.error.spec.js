import { createRouter, createStore, mount } from '../../helpers';
import biometricsRegistrationError from '@/pages/biometrics-registration/error';
import each from 'jest-each';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import NativeApp from '@/services/native-app';

jest.mock('@/lib/utils');

describe('biometrics registration error page', () => {
  let wrapper;
  let store;
  let $router;

  const mountPage = ({
    biometricType = 'face',
    isNativeApp = true,
    errorCode = undefined,
    source = 'ios',
  } = {}) => {
    $router = createRouter();
    store = createStore({
      state: {
        loginSettings: {
          biometricType,
          errorCode,
        },
        appVersion: {
          webVersion: '1.2.3',
          nativeVersion: '3.2.1',
        },
        device: {
          isNativeApp,
          source,
        },
      },
      getters: {
        'loginSettings/biometricError': errorCode,
        'loginSettings/biometricType': biometricType,
      },
    });
    wrapper = mount(biometricsRegistrationError,
      { $store: store,
        data: () => ({
          cannotFindErrorText: `loginSettings.biometrics.errors.cannotFindBiometricType.errorText.${biometricType}`,
        }),
        $router,
        stubs: {
          'no-return-flow-layout': '<div><slot/></div>',
        },
      });
  };

  describe('biometric registration error screen', () => {
    describe('cannot change biometrics', () => {
      each([
        ['have information showing Face ID could not be changed', 'ios', 'face', 'We cannot turn on Face ID due to a technical problem.', 'If you still cannot turn on Face ID,', 'If you cannot turn on Face ID, you\'ll need to log in using your email, password and security code.'],
        ['have information showing Touch ID could not be changed', 'ios', 'touch', 'We cannot turn on Touch ID due to a technical problem.', 'If you still cannot turn on Touch ID,', 'If you cannot turn on Touch ID, you\'ll need to log in using your email, password and security code.'],
        ['have information showing Fingerprint could not be changed', 'android', 'fingerPrintFaceOrIris', 'We cannot turn on fingerprint, face or iris recognition (biometrics) due to a technical problem.', 'If you still cannot turn on biometrics,', 'If you cannot turn on biometrics, you\'ll need to log in using your email, password and security code.'],
      ]).it('will %s', (_, source, biometricType, text1, text2, text3) => {
        mountPage(
          {
            errorCode: biometricErrorCodes.CannotChangeBiometrics,
            biometricType,
            source,
          },
        );
        expect(wrapper.findAll('p').at(0).text()).toContain(text1);
        expect(wrapper.findAll('p').at(1).text()).toContain('Try again later.');
        expect(wrapper.findAll('p').at(2).text()).toContain(text2);
        expect(wrapper.findAll('p').at(3).text()).toContain(text3);
        expect(wrapper.findAll('a').at(0).text()).toContain('get help with logging in using biometrics');
        expect(wrapper.find('a').element.getAttribute('href')).toContain('https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/logging-in-to-the-nhs-app/');
      });
    });

    describe('cannot find biometrics', () => {
      each([
        ['have information showing Face ID could not be found', 'ios', 'face', 'turned on Face ID', 'added a face scan', 'If you cannot use Face ID, you\'ll need to log in using your email, password and security code.'],
        ['have information showing Touch ID could not be found', 'ios', 'touch', 'turned on Touch ID', 'added a fingerprint scan', 'If you cannot use Touch ID, you\'ll need to log in using your email, password and security code.'],
        ['have information showing Fingerprint could not be found', 'android', 'fingerPrintFaceOrIris', 'turned on biometrics', 'added a fingerprint, face or iris scan', 'If you cannot use biometrics, you\'ll need to log in using your email, password and security code.'],
      ]).it('will %s', (_, source, biometricType, listItem1, listItem2, text1) => {
        mountPage(
          {
            errorCode: biometricErrorCodes.CannotFindBiometrics,
            biometricType,
            source,
          },
        );

        expect(wrapper.findAll('p').at(0).text()).toContain('Check your device settings to make sure you have:');
        expect(wrapper.findAll('li').at(0).text()).toContain(listItem1);
        expect(wrapper.findAll('li').at(1).text()).toContain(listItem2);
        expect(wrapper.findAll('p').at(1).text()).toContain(text1);
      });
    });

    describe('undefined errorCode', () => {
      it('will redirect to login settings', async () => {
        NativeApp.goToLoggedInHomeScreen = jest.fn().mockImplementation(() => true);
        mountPage();
        expect(NativeApp.goToLoggedInHomeScreen).toBeCalled();
      });
    });
  });
});
