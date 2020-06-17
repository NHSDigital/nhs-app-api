import Settings from '@/components/account/Settings';
import { ACCOUNT_NOTIFICATIONS_PATH, LOGIN_SETTINGS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { mount, createStore } from '../../helpers';

jest.mock('@/lib/utils');

describe('Settings', () => {
  let wrapper;
  let $store;

  const mountSettings = ({
    showBiometrics = true,
    showNotifications = true,
    showLinkedProfiles = true,
    source = 'ios',
    versionEnabled = true } = {}) => {
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
      },
    });
    return mount(Settings, {
      methods: {
        configureWebContext(url) {
          return url;
        },
      },
      $store,
      propsData: {
        showBiometrics,
        showNotifications,
        showLinkedProfiles },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('biometrics, notifications and linked profiles enabled', () => {
    let notificationsLink;
    let biometricsLink;

    beforeEach(() => {
      wrapper = mountSettings();
      notificationsLink = wrapper.find('#btn_notificationOptions');
      biometricsLink = wrapper.find('#btn_passwordOptions');
    });

    it('can see Biometrics', () => {
      expect(biometricsLink.exists()).toBe(true);
    });

    it('can see Notifications', () => {
      expect(notificationsLink.exists()).toBe(true);
    });

    it('notifications has correct text', () => {
      expect(notificationsLink.text()).toEqual('translate_myAccount.accountSettings.notificationOptions');
    });

    it('the notifications link will direct to notifications page', () => {
      global.digitalData = {};
      notificationsLink.trigger('click');
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ACCOUNT_NOTIFICATIONS_PATH);
    });
  });

  describe('biometrics enabled, linked profiles enabled and notifications disabled', () => {
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

  describe('biometrics disabled, linked profiles disabled and notifications enabled', () => {
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
    describe('biometrics web enabled', () => {
      let biometricsLink;

      beforeEach(() => {
        wrapper = mountSettings();
        biometricsLink = wrapper.find('#btn_passwordOptions');
      });

      it('will navigate to the web biometrics', () => {
        biometricsLink.trigger('click');
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
      });
    });

    describe('biometrics web disabled', () => {
      describe('android', () => {
        let biometricsLink;

        beforeEach(() => {
          wrapper = mountSettings({ source: 'android' });
          biometricsLink = wrapper.find('#btn_passwordOptions');
        });

        it('will navigate to the web biometrics', () => {
          biometricsLink.trigger('click');
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
        });
      });

      describe('ios', () => {
        let biometricsLink;

        beforeEach(() => {
          wrapper = mountSettings();
          biometricsLink = wrapper.find('#btn_passwordOptions');
        });

        it('will navigate to the web biometrics', () => {
          biometricsLink.trigger('click');
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, LOGIN_SETTINGS_PATH);
        });
      });
    });
  });
});
