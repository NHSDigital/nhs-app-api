import i18n from '@/plugins/i18n';
import AccountAndSettings from '@/components/more/account-and-settings/AccountAndSettings';
import {
  MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH,
  MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH,
  MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH,
  MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { mount, createStore } from '../../helpers';

jest.mock('@/lib/utils');

describe('AccountAndSettings', () => {
  let wrapper;
  let $store;

  const mountSettings = ({
    showBiometrics = true,
    biometricSupported = false,
    biometricType = 'fingerPrint',
    showNotifications = true,
    source = 'ios',
    versionEnabled = true,
  } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
          source,
        },
        session: {
          isLoggedIn: jest.fn(),
        },
      },
      getters: {
        'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(versionEnabled),
        'loginSettings/biometricType': biometricType,
        'loginSettings/biometricSupported': biometricSupported,
        'knownServices/matchOneById': id => ({
          id,
          url: 'www.url.com',
        }),
      },
    });

    $store.app.$http = {
      postV1PatientAssertedLoginIdentity: jest.fn()
        .mockImplementation(() => Promise.resolve({ token: 'jwtToken' })),
    };
    return mount(AccountAndSettings, {
      $store,
      propsData: {
        showBiometrics,
        showNotifications,
      },
      mountOpts: { i18n },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('biometrics and notifications enabled', () => {
    let notificationsLink;
    let biometricsLink;
    let nhsLoginLink;

    beforeEach(() => {
      wrapper = mountSettings();
      notificationsLink = wrapper.find('#btn_notificationOptions');
      biometricsLink = wrapper.find('#btn_passwordOptions');
      nhsLoginLink = wrapper.find('#btn_nhsLogin');
    });

    it('can see Biometrics', () => {
      expect(biometricsLink.exists()).toBe(true);
    });

    it('can see NHS Login', () => {
      expect(nhsLoginLink.exists()).toBe(true);
    });

    it('NHS login has the correct text', () => {
      expect(nhsLoginLink.text()).toBe('Manage NHS login account');
    });

    it('can see Notifications', () => {
      expect(notificationsLink.exists()).toBe(true);
    });

    it('notifications has correct text', () => {
      expect(notificationsLink.text()).toEqual('Manage notifications');
    });

    it('the notifications link will direct to notifications page', () => {
      global.digitalData = {};
      notificationsLink.trigger('click');
      expect(redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, MORE_ACCOUNTANDSETTINGS_MANAGENOTIFICATIONS_PATH);
    });
  });

  describe('biometrics enabled and notifications disabled', () => {
    let notificationsLink;
    let biometricsLink;

    beforeEach(() => {
      wrapper = mountSettings({ showBiometrics: true, showNotifications: false });
      notificationsLink = wrapper.find('#btn_notificationOptions');
      biometricsLink = wrapper.find('#btn_passwordOptions');
    });

    it('can see Biometrics', () => {
      expect(biometricsLink.exists()).toBe(true);
    });

    it('cannot see Notifications', () => {
      expect(notificationsLink.exists()).toBe(false);
    });
  });

  describe('biometrics disabled and notifications enabled', () => {
    let notificationsLink;
    let biometricsLink;

    beforeEach(() => {
      wrapper = mountSettings({ showBiometrics: false, showNotifications: true });
      notificationsLink = wrapper.find('#btn_notificationOptions');
      biometricsLink = wrapper.find('#btn_passwordOptions');
    });

    it('cannot see Biometrics', () => {
      expect(biometricsLink.exists()).toBe(false);
    });

    it('can see Notifications', () => {
      expect(notificationsLink.exists()).toBe(true);
    });
  });

  describe('Biometric Link', () => {
    let biometricsLink;

    describe('biometrics web enabled with fingerprint enabled', () => {
      beforeEach(() => {
        wrapper = mountSettings({
          biometricType: 'fingerPrint',
          biometricSupported: true,
        });
        biometricsLink = wrapper.find('#btn_passwordOptions');
      });

      it('will navigate to the fingerprint biometrics', () => {
        expect(biometricsLink.text()).toBe('Fingerprint');
        global.digitalData = {};
        biometricsLink.trigger('click');
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm,
          MORE_ACCOUNTANDSETTINGS_FINGERPRINT_PATH);
      });
    });

    describe('biometrics web enabled with face id enabled', () => {
      beforeEach(() => {
        wrapper = mountSettings({
          biometricType: 'face',
          biometricSupported: true,
        });
        biometricsLink = wrapper.find('#btn_passwordOptions');
      });

      it('will navigate to the face id biometrics', () => {
        expect(biometricsLink.text()).toBe('Face ID');
        global.digitalData = {};
        biometricsLink.trigger('click');
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm,
          MORE_ACCOUNTANDSETTINGS_FACE_ID_PATH);
      });
    });

    describe('biometrics web enabled with touch id enabled', () => {
      beforeEach(() => {
        wrapper = mountSettings({
          biometricType: 'touch',
          biometricSupported: true,
        });
        biometricsLink = wrapper.find('#btn_passwordOptions');
      });

      it('will navigate to the face id biometrics', () => {
        expect(biometricsLink.text()).toBe('Touch ID');
        global.digitalData = {};
        biometricsLink.trigger('click');
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm,
          MORE_ACCOUNTANDSETTINGS_TOUCH_ID_PATH);
      });
    });

    describe('biometrics web disabled', () => {
      describe('android', () => {
        beforeEach(() => {
          wrapper = mountSettings({ source: 'android' });
          biometricsLink = wrapper.find('#btn_passwordOptions');
        });

        it('will navigate to the web biometrics', () => {
          global.digitalData = {};
          expect(biometricsLink.text()).toBe('Login options');
          biometricsLink.trigger('click');
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm,
            MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH);
        });
      });

      describe('ios', () => {
        beforeEach(() => {
          wrapper = mountSettings();
          biometricsLink = wrapper.find('#btn_passwordOptions');
        });

        it('will navigate to the web biometrics', () => {
          expect(biometricsLink.text()).toBe('Login options');
          global.digitalData = {};
          biometricsLink.trigger('click');
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm,
            MORE_ACCOUNTANDSETTINGS_LOGINOPTIONS_PATH);
        });
      });
    });
  });
});
