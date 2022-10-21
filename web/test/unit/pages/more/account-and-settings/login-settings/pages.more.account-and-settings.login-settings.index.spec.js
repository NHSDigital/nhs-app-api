import loginSettings from '@/pages/more/account-and-settings/login-settings/index';
import { mount, createRouter, createStore } from '../../../../helpers';

describe('login settings page', () => {
  let wrapper;
  let $store;
  let $router;

  const mountPage = ({
    biometricsRegistrationStatus = false,
    biometricType = 'face',
    isNativeApp = true,
    hasBiometricType = true,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        loginSettings: {
          biometricsRegistrationStatus,
          biometricType,
        },
        device: {
          isNativeApp,
        },
      },
      getters: {
        'loginSettings/biometricType': biometricType,
        'loginSettings/biometricRegistered': biometricsRegistrationStatus,
        'loginSettings/biometricSupported': hasBiometricType,
      },
    });
    wrapper = mount(loginSettings, { $store, $router });
  };

  describe('biometric toggle', () => {
    describe('biometric type undefined', () => {
      beforeEach(() => {
        mountPage({ biometricType: undefined, hasBiometricType: false });
      });

      it('will have information showing biometric type could not be found', () => {
        expect(wrapper.findAll('p').at(0).text()).toContain('We cannot find any fingerprint or face recognition settings on your device.');
      });
    });

    describe('biometric type defined as FaceID', () => {
      it('will have information showing biometric type could be found', () => {
        expect(wrapper.findAll('p').at(0).text()).toContain('We cannot find any fingerprint or face recognition settings on your device.');
      });

      describe('biometric registration disabled', () => {
        beforeEach(() => {
          mountPage();
        });

        it('will be unchecked if registration is disabled', () => {
          const toggle = wrapper.find('input');
          expect(toggle.element.checked).toBeFalsy();
        });
      });

      describe('biometric registration enabled', () => {
        beforeEach(() => {
          mountPage({ biometricsRegistrationStatus: true });
        });

        it('will be checked if registration is enabled', () => {
          const toggle = wrapper.find('input');
          expect(toggle.element.checked).toBeTruthy();
        });
      });

      describe('biometric registration action, when banner has been dismissed', () => {
        beforeEach(() => {
          mountPage();
        });

        it('will dispatch `loginSettings/updateRegistrationStatus` when clicked',
          () => {
            wrapper.find('label').trigger('click');
            expect($store.dispatch).toHaveBeenLastCalledWith('loginSettings/updateRegistration');
          });
      });

      describe('biometric registration action, when banner has not been dismissed', () => {
        beforeEach(() => {
          mountPage({ biometricsBannerDismissed: false });
          wrapper.find('label').trigger('click');
        });

        it('will dispatch `loginSettings/updateRegistrationStatus` when clicked',
          () => {
            expect($store.dispatch).toHaveBeenCalledWith('loginSettings/updateRegistration');
          });
      });
    });
    describe('page information text', () => {
      describe('device biometric type is face', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'face' });
        });

        it('will show the correct information text',
          () => {
            const label = wrapper.find('#biometricInformation');
            expect(label.element.textContent.trim()).toEqual('Face ID lets you log in with your face scan instead of a password and security code.');
          });
      });
      describe('device biometric type is fingerPrint or iris', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'fingerPrintFaceOrIris' });
        });

        it('will show the correct information text',
          () => {
            const label = wrapper.find('#biometricInformation');
            expect(label.element.textContent.trim()).toEqual('You can log in with your fingerprint, face or iris instead of a password and security code if your device meets Google\'s increased security settings.');
          });
      });
      describe('device biometric type is touch', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'touch' });
        });

        it('will show the correct information text',
          () => {
            const label = wrapper.find('#biometricInformation');
            expect(label.element.textContent.trim()).toEqual('Touch ID lets you log in with your fingerprint instead of a password and security code.');
          });
      });
    });
    describe('toggle label', () => {
      describe('biometric type is face', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'face' });
        });

        it('will show the correct biometric type',
          () => {
            const label = wrapper.find('strong');
            expect(label.element.textContent).toEqual('Log in with Face ID');
          });
      });
      describe('biometric type is touch', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'touch' });
        });

        it('will show the correct biometric type',
          () => {
            const label = wrapper.find('strong');
            expect(label.element.textContent).toEqual('Log in with Touch ID');
          });
      });
      describe('biometric type is fingerprint, face or iris', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'fingerPrintFaceOrIris' });
        });

        it('will show the correct biometric type',
          () => {
            const label = wrapper.find('strong');
            expect(label.element.textContent).toEqual('Log in with fingerprint, face or iris');
          });
      });
    });
  });
});
