import Notifications from '@/pages/notifications/index';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';
import { createRouter, createStore, mount } from '../../helpers';
import { BIOMETRICS_REGISTRATION_PATH } from '@/router/paths';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('notifications prompt page', () => {
  let $router;
  let $store;
  let wrapper;
  let conditionalRedirect;

  const mountPage = ({ state }) => {
    $router = createRouter();
    $store = createStore({ state });
    conditionalRedirect = jest.fn();

    return mount(Notifications, {
      $router,
      $store,
      stubs: {
        'no-return-flow-layout': '<div><slot/></div>',
      },
      methods: {
        conditionalRedirect,
      },
    });
  };

  beforeEach(() => {
    EventBus.$emit.mockClear();
  });

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
            notificationCommunicationError: false,
          },
        },
      });
    });

    describe('the user does not make a choice', () => {
      beforeEach(() => {
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will show inline error', () => {
        expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(true);
      });

      it('will show form error summary', () => {
        expect(wrapper.find('#form-error-summary').exists()).toBe(true);
      });

      it('will not dispatch', () => {
        expect($store.dispatch).not.toBeCalled();
      });

      it('will not call conditional redirect', () => {
        expect(conditionalRedirect).not.toBeCalled();
      });

      it('will set focus on the error component', () => {
        expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
      });

      describe('choice made', () => {
        beforeEach(() => {
          wrapper.find('#notifications-yes').setChecked();
        });

        it('will not show inline error', () => {
          expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(false);
        });

        it('will not show error dialog', () => {
          expect(wrapper.find('#message-dialog').exists()).toBe(false);
        });
      });
    });

    describe('the user agreed to notifications', () => {
      beforeEach(() => {
        wrapper.find('#notifications-yes').setChecked();
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will not show inline error', () => {
        expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(false);
      });

      it('will not show error dialog', () => {
        expect(wrapper.find('#message-dialog').exists()).toBe(false);
      });

      it('will not set focus on the error component', () => {
        expect(EventBus.$emit).not.toBeCalled();
      });

      it('will call to native to toggle permission', () => {
        expect($store.dispatch).toBeCalledWith('notifications/toggle');
      });

      it('will add the cookie', () => {
        expect($store.dispatch).toBeCalledWith('notifications/addNotificationCookie');
      });

      it('will not log metrics', () => {
        expect($store.dispatch).not.toBeCalledWith('notifications/logMetrics', expect.anything);
      });

      it('will log audit', () => {
        expect($store.dispatch).toBeCalledWith('notifications/logAudit', {
          notificationsRegistered: true,
          notificationsDecisionSource: 'Prompt',
        });
      });

      it('will call conditional redirect', () => {
        expect($router.push).toBeCalledWith({ path: BIOMETRICS_REGISTRATION_PATH });
      });
    });

    describe('the user does not agree to notifications', () => {
      beforeEach(() => {
        wrapper.find('#notifications-no').setChecked();
        wrapper.find('#btn_continue').trigger('click');
      });

      it('will not show inline error', () => {
        expect(wrapper.find('.nhsuk-form-group--error').exists()).toBe(false);
      });

      it('will not show error dialog', () => {
        expect(wrapper.find('#message-dialog').exists()).toBe(false);
      });

      it('will not set focus on the error component', () => {
        expect(EventBus.$emit).not.toBeCalled();
      });

      it('will add the cookie', () => {
        expect($store.dispatch).toBeCalledWith('notifications/addNotificationCookie');
      });

      it('will log metrics', () => {
        expect($store.dispatch).toBeCalledWith('notifications/logMetrics', {
          screenShown: true,
          notificationsRegistered: false,
          didErrorAttemptingToUpdateStatus: false,
          ignoreError: true,
        });
      });

      it('will log audit', () => {
        expect($store.dispatch).toBeCalledWith('notifications/logAudit', {
          notificationsRegistered: false,
          notificationsDecisionSource: 'Prompt',
        });
      });

      it('will call conditional redirect', () => {
        expect($router.push).toBeCalledWith({ path: BIOMETRICS_REGISTRATION_PATH });
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
          ignoreError: true,
        });
      });

      it('will call conditional redirect', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });
  });

  describe('an error has occurred in notifications/load in middleware', () => {
    beforeEach(() => {
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
          notifications: {
            notificationCookieExists: false,
            registered: false,
            notificationCommunicationError: true,
          },
        },
      });
    });

    describe('the user gets to a point where the screen would load', () => {
      it('will not add the cookie', () => {
        expect($store.dispatch).not.toBeCalledWith('notifications/addNotificationCookie');
      });

      it('will log metrics', () => {
        expect($store.dispatch).toBeCalledWith('notifications/logMetrics', {
          screenShown: false,
          notificationsRegistered: false,
          didErrorAttemptingToUpdateStatus: false,
          ignoreError: true,
        });
      });

      it('will call conditional redirect', () => {
        expect(conditionalRedirect).toBeCalled();
      });
    });
  });
});
