import loginSettings from '@/pages/account/login-settings/';
import { mount, createRouter, createStore } from '../../helpers';

describe('login settings page', () => {
  let wrapper;
  let $store;
  let $router;

  const mountPage = ({
    biometricsRegistrationStatus = false,
    biometricType = 'loginSettings.biometrics.biometricType.face',
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
        'loginSettings/deviceBiometricType': biometricType,
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
        expect(wrapper.findAll('p').at(0).text()).toContain('translate_loginSettings.biometrics.noBiometricType.information.paragraph1');
      });
    });

    describe('biometric type defined as FaceID', () => {
      it('will have information showing biometric type could be found', () => {
        expect(wrapper.findAll('p').at(0).text()).toContain('translate_loginSettings.biometrics.noBiometricType.information.paragraph1');
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
            expect($store.dispatch).toBeCalledWith('loginSettings/updateRegistrationStatus');
          });
      });

      describe('label', () => {
        beforeEach(() => {
          mountPage({ biometricType: 'loginSettings.biometrics.biometricTyp.fingerPrint' });
        });

        it('will show the correct biometric type',
          () => {
            const label = wrapper.find('strong');
            expect(label.element.textContent).toEqual('translate_loginSettings.biometrics.toggleLabel');
          });
      });
    });
  });
});
