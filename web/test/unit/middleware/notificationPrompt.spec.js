import notificationPrompt from '@/middleware/notificationsPrompt';
import { NOTIFICATIONS_PATH } from '@/router/paths';
import { INDEX_NAME, NOTIFICATIONS_NAME } from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

describe('notification prompt next', () => {
  const params = 'params';
  const query = 'query';
  let next;
  let store;

  const callsNotificationsPrompt = ({ isNativeApp }) => {
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
      },
      state: {
        device: {
          isNativeApp,
        },
      },
    });
    notificationPrompt({ next, to, store });
  };

  describe('native', () => {
    beforeEach(() => {
      callsNotificationsPrompt({ isNativeApp: true });
    });

    it('will dispatch `notifications/load`', () => {
      expect(store.dispatch).toBeCalledWith('notifications/load');
    });

    it('will dispatch `notifications/load`', () => {
      expect(store.dispatch).toBeCalledWith('notifications/checkNotificationCookie');
    });

    it('will call next', () => {
      expect(next).toBeCalled();
    });
  });

  describe('not on native platform', () => {
    const redirectRoute = 'redirectRoute';

    beforeEach(() => {
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
});
