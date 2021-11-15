import Notifications from '@/pages/notifications/index';
import { createStore, mount } from '../../helpers';

describe('notifications prompt page', () => {
  let $store;
  let wrapper;
  let conditionalRedirect;

  const mountPage = ({ state }) => {
    $store = createStore({ state });
    conditionalRedirect = jest.fn();

    return mount(Notifications, {
      $store,
      stubs: {
        'no-return-flow-layout': '<div><slot/></div>',
      },
      methods: {
        conditionalRedirect,
      },
    });
  };

  describe('new user is prompted for notifications', () => {
    beforeEach(() => {
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
          notifications: {
            notificationCookieExists: false,
            registered: false,
            toggleUpdated: false,
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
        expect($store.dispatch).toBeCalledWith('notifications/addNotificationCookie');
      });

      it('will call conditional redirect', () => {
        expect(conditionalRedirect).toBeCalled();
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
        expect($store.dispatch).toBeCalledWith('notifications/addNotificationCookie');
      });

      it('will log metrics', () => {
        expect($store.dispatch).toBeCalledWith('notifications/logMetrics', {
          screenShown: true,
          notificationsRegistered: false,
          didErrorAttemptingToUpdateStatus: false,
        });
      });

      it('will call conditional redirect', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });
  });

  describe('the user is existing and already had notifications in place so they should not see this screen', () => {
    beforeEach(() => {
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
        expect($store.dispatch).toBeCalledWith('notifications/addNotificationCookie');
      });

      it('will log metrics', () => {
        expect($store.dispatch).toBeCalledWith('notifications/logMetrics', {
          screenShown: false,
          notificationsRegistered: true,
          didErrorAttemptingToUpdateStatus: false,
        });
      });

      it('will call conditional redirect', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });
  });
});
