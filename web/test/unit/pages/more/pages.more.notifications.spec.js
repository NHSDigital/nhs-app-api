import Notifications from '@/pages/more/notifications';
import { initialState } from '@/store/modules/notifications/mutation-types';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/native-app');

describe('notifications', () => {
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
