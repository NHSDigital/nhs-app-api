import Notifications from '@/pages/more/account-and-settings/manage-notifications';
import { initialState } from '@/store/modules/notifications/mutation-types';
import { createStore, mount } from '../../../helpers';
import { redirectTo } from '@/lib/utils';
import {
  MORE_ACCOUNTANDSETTINGS_EXAMPLE_NOTIFICATIONS_PATH,
  MORE_ACCOUNTANDSETTINGS_MORETHAN_ONE_DEVICE_PATH,
} from '@/router/paths';

jest.mock('@/lib/utils');
jest.mock('@/services/native-app');

describe('manage notifications', () => {
  let wrapper;
  let $store;
  let state;

  beforeEach(() => {
    state = {
      notifications: initialState(),
      device: {
        isNativeApp: true,
      },
    };

    $store = createStore({ state });

    redirectTo.mockClear();

    wrapper = mount(Notifications, {
      $store,
    });
  });

  it('will dispatch `notifications/toggle` when text label is clicked', () => {
    wrapper.find('label').trigger('click');
    expect($store.dispatch).toBeCalledWith('notifications/toggle');
  });

  it('will dispatch `notifications/toggle` when UI label is clicked', () => {
    wrapper.findAll('span').trigger('click');
    expect($store.dispatch).toBeCalledWith('notifications/toggle');
  });

  it('will dispatch `notifications/logAudit` when text label is clicked', () => {
    wrapper.find('label').trigger('click');
    expect($store.dispatch).toBeCalledWith('notifications/logAudit', {
      notificationsRegistered: true,
      notificationsDecisionSource: 'Toggle',
    });
  });

  it('will dispatch `notifications/logAudit` when UI label is clicked', () => {
    wrapper.findAll('span').trigger('click');
    expect($store.dispatch).toBeCalledWith('notifications/logAudit', {
      notificationsRegistered: true,
      notificationsDecisionSource: 'Toggle',
    });
  });

  it('will redirect to example notification page when function navigateToExampleNotifications is called', () => {
    wrapper.vm.navigateToExampleNotifications();
    expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, MORE_ACCOUNTANDSETTINGS_EXAMPLE_NOTIFICATIONS_PATH);
  });

  it('will redirect to more than one device page when function navigateToNotificationsToMoreThanOneDevice is called ', () => {
    wrapper.vm.navigateToNotificationsToMoreThanOneDevice();
    expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, MORE_ACCOUNTANDSETTINGS_MORETHAN_ONE_DEVICE_PATH);
  });

  describe('created', () => {
    it('will dispatch `notifications/load`', () => {
      expect($store.dispatch).toBeCalledWith('notifications/load');
    });
  });

  describe('toggle', () => {
    let toggle;

    beforeEach(() => {
      toggle = wrapper.find('input');
    });

    it('will not be active by default', () => {
      expect(toggle.element.checked).toBe(false);
    });

    it('will be active when registered', () => {
      state.notifications.registered = true;
      expect(toggle.element.checked).toBe(true);
    });
  });
});
