import Settings from '@/components/account/Settings';
import { ACCOUNT_NOTIFICATIONS } from '@/lib/routes';
import { createRouter, mount } from '../../helpers';

describe('Settings', () => {
  let $router;
  let wrapper;

  const mountSettings = ({ showBiometrics = true, showNotifications = true } = {}) => {
    $router = createRouter();
    return mount(Settings, {
      $router,
      propsData: { showBiometrics, showNotifications },
    });
  };

  describe('biometrics and notifications enabled', () => {
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
      expect($router.push).toHaveBeenCalledWith(ACCOUNT_NOTIFICATIONS.path);
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
});
