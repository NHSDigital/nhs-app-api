import Notifications from '@/pages/notifications/index';
import { APPOINTMENTS_NAME, REDIRECT_PARAMETER, ACCOUNT_NOTIFICATIONS_NAME } from '@/router/names';
import { INDEX_PATH, ACCOUNT_NOTIFICATIONS_PATH } from '@/router/paths';
import * as LibUtils from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

LibUtils.redirectTo = jest.fn();
LibUtils.redirectByName = jest.fn();

describe('notifications prompt page', () => {
  let $store;
  let wrapper;
  let $router;

  const mountPage = ({ state, query }) => {
    $router = createRouter();
    $store = createStore({ state });

    return mount(Notifications, {
      $store,
      stubs: {
        'no-return-flow-layout': '<div><slot/></div>',
      },
      $route: {
        path: ACCOUNT_NOTIFICATIONS_PATH,
        name: ACCOUNT_NOTIFICATIONS_NAME,
        query,
        currentRoute: {
          path: ACCOUNT_NOTIFICATIONS_PATH,
        },
      },
      $router,
    });
  };

  describe('content', () => {
    LibUtils.redirectTo.mockClear();
    LibUtils.redirectByName.mockClear();
    beforeEach(() => {
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
          notifications: {
            notificationCookieExists: false,
          },
        },
      });
    });

    it('will show the expected paragraphs', () => {
      const paragraphs = wrapper.findAll('p');
      expect(paragraphs.at(0).text()).toEqual('These may include new features and public health updates.');
      expect(paragraphs.at(1).text()).toContain('If you share this device with other people, they may see your notifications. The settings will apply to everyone who logs in to the NHS App on this device.');
      expect(paragraphs.at(2).text()).toContain('More information is available in the NHS App privacy policy.');
    });

    it('will have the correct text for radio labels', () => {
      wrapper.findAll('label');
      expect(wrapper.findAll('label').at(0).text()).toEqual('Allow notifications  I accept the NHS App sending notifications on this device');
    });
  });

  describe('new user is prompted for notifications', () => {
    beforeEach(() => {
      LibUtils.redirectTo.mockClear();
      LibUtils.redirectByName.mockClear();
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
          notifications: {
            notificationCookieExists: false,
            registered: false,
          },
        },
      });
    });

    describe('the user agreed to notifications', () => {
      beforeEach(() => {
        wrapper.find('#allow_notifications').trigger('click');
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will call to native to toggle permission', () => {
        expect($store.dispatch).toBeCalledWith('notifications/toggle');
      });

      it('will add the cookie', () => {
        expect($store.dispatch).toHaveBeenCalledWith('notifications/addNotificationCookie');
      });
    });

    describe('the user does not agree to notifications', () => {
      beforeEach(() => {
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will not call to native to toggle permission', () => {
        expect($store.dispatch).not.toBeCalledWith('notifications/toggle');
      });

      it('will add the cookie', () => {
        expect($store.dispatch).toHaveBeenCalledWith('notifications/addNotificationCookie');
      });
    });
  });

  describe('current route has a redirect query parameter', () => {
    beforeEach(() => {
      LibUtils.redirectTo.mockClear();
      LibUtils.redirectByName.mockClear();
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
          notifications: {
            notificationCookieExists: false,
            registered: false,
          },
        },
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS_NAME },
      });
    });

    describe('the user agrees to notifications and continues', () => {
      beforeEach(() => {
        wrapper.find('#allow_notifications').trigger('click');
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will redirect to redirect parameter route', () => {
        expect(LibUtils.redirectByName).toBeCalledWith(wrapper.vm, APPOINTMENTS_NAME);
      });
    });
  });

  describe('the user is existing and already had notifications in place so they should not see this screen', () => {
    beforeEach(() => {
      LibUtils.redirectTo.mockClear();
      LibUtils.redirectByName.mockClear();
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
          notifications: {
            notificationCookieExists: false,
            registered: true,
          },
        },
      });
    });

    describe('the user gets to a point where the screen would load', () => {
      it('will add the cookie', () => {
        expect($store.dispatch).toHaveBeenCalledWith('notifications/addNotificationCookie');
      });

      it('will redirect to the index page', () => {
        expect(LibUtils.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
      });
    });
  });
});
