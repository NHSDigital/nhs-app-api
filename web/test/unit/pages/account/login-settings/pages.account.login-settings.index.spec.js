import loginSettings from '@/pages/account/login-settings/index';
import { mount, createRouter, createStore } from '../../../helpers';

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
        'loginSettings/biometricState': biometricsRegistrationStatus,
        'loginSettings/getBiometricToggleText': `loginSettings.biometrics.toggleLabel.${biometricType}`,
        'loginSettings/getDeviceBiometricNameString': `loginSettings.biometrics.biometricType.${biometricType}`,
      },
    });
    wrapper = mount(loginSettings,
      { $store,
        data: () => ({
          hasBiometricType,
        }),
        $router });
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

      describe('biometric registration action', () => {
        beforeEach(() => {
          mountPage();
        });

        it('will dispatch `loginSettings/updateRegistrationStatus` when clicked',
          () => {
            wrapper.find('label').trigger('click');
            expect($store.dispatch).toHaveBeenLastCalledWith('loginSettings/updateRegistration');
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
      describe('biometric type is fingerPrint', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'fingerPrint' });
        });

        it('will show the correct biometric type',
          () => {
            const label = wrapper.find('strong');
            expect(label.element.textContent).toEqual('Log in with fingerprint');
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
    });
  });
});
