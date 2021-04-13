import notificationPrompt from '@/middleware/notificationsPrompt';
import { NOTIFICATIONS_PATH } from '@/router/paths';
import { INDEX_NAME, NOTIFICATIONS_NAME } from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

describe('notification prompt next', () => {
  const params = 'params';
  const query = 'query';
  let isNativeApp;
  let next;
  let notificationPromptEnabled;
  let store;

  const callsNotificationsPrompt = ({ notificationCookieExists = false } = {}) => {
    const to = {
      name: NOTIFICATIONS_NAME,
      params,
      path: NOTIFICATIONS_PATH,
      query,
    };
    next = jest.fn();
    store = createStore({
      getters: {
        'session/isLoggedIn': jest.fn().mockReturnValue(true),
        'serviceJourneyRules/notificationPromptEnabled': notificationPromptEnabled,
      },
      state: {
        device: {
          isNativeApp,
        },
        notifications: {
          notificationCookieExists,
        },
      },
    });
    notificationPrompt({ next, to, store });
  };

  describe('native', () => {
    beforeEach(() => {
      isNativeApp = true;
    });

    describe('notification prompt disabled', () => {
      const redirectRoute = 'redirectRoute';

      beforeEach(() => {
        notificationPromptEnabled = false;
        createConditionalRedirectRouteByName.mockReturnValue(redirectRoute);
        callsNotificationsPrompt();
      });

      it('will not dispatch any actions', () => {
        expect(store.dispatch).not.toBeCalled();
      });

      it('will create redirect route to INDEX', () => {
        expect(createConditionalRedirectRouteByName).toBeCalledWith({
          name: INDEX_NAME,
          params,
          query,
          store,
        });
      });

      it('will call next with redirect route', () => {
        expect(next).toBeCalledWith(redirectRoute);
      });
    });

    describe('notification prompt enabled', () => {
      beforeEach(() => {
        notificationPromptEnabled = true;
      });

      describe('notification cookie exists', () => {
        beforeEach(() => {
          callsNotificationsPrompt({ notificationCookieExists: true });
        });

        it('will dispatch `notifications/checkNotificationCookie`', () => {
          expect(store.dispatch).toBeCalledWith('notifications/checkNotificationCookie');
        });

        it('will not dispatch `notifications/load`', () => {
          expect(store.dispatch).not.toBeCalledWith('notifications/load');
        });

        it('will call next', () => {
          expect(next).toBeCalled();
        });
      });

      describe('notification cookie does not exist', () => {
        beforeEach(() => {
          callsNotificationsPrompt({ notificationCookieExists: false });
        });

        it('will dispatch `notifications/checkNotificationCookie`', () => {
          expect(store.dispatch).toBeCalledWith('notifications/checkNotificationCookie');
        });

        it('will dispatch `notifications/load`', () => {
          expect(store.dispatch).toBeCalledWith('notifications/load');
        });

        it('will call next', () => {
          expect(next).toBeCalled();
        });
      });
    });
  });

  describe('not on native platform', () => {
    const redirectRoute = 'redirectRoute';

    beforeEach(() => {
      isNativeApp = false;
      createConditionalRedirectRouteByName.mockReturnValue(redirectRoute);
      callsNotificationsPrompt({ isNativeApp: false });
    });

    it('will not dispatch any actions', () => {
      expect(store.dispatch).not.toBeCalled();
    });

    it('will create redirect route to INDEX', () => {
      expect(createConditionalRedirectRouteByName).toBeCalledWith({
        name: INDEX_NAME,
        params,
        query,
        store,
      });
    });

    it('will call next with redirect route', () => {
      expect(next).toBeCalledWith(redirectRoute);
    });
  });

  afterEach(() => {
    notificationPromptEnabled = undefined;
    isNativeApp = undefined;
  });
});
