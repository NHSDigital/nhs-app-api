import notificationPrompt from '@/middleware/notificationsPrompt';
import * as dependency from '@/lib/utils';
import { NOTIFICATIONS_PATH } from '@/router/paths';
import { INDEX_NAME, NOTIFICATIONS_NAME } from '@/router/names';

describe('notification prompt next', () => {
  let nativeVersionIsSupported = true;
  let next;
  let store;
  dependency.createRoutePathObject = jest.fn(x => ({ path: x.path }));

  const callsNotificationsPrompt = () => {
    const to = {
      name: NOTIFICATIONS_NAME,
      path: NOTIFICATIONS_PATH,
    };
    notificationPrompt({ next, to, store });
  };

  describe('Native', () => {
    beforeEach(() => {
      next = jest.fn();
      store = {
        getters: {
          'session/isLoggedIn': jest.fn().mockReturnValue(true),
          'appVersion/isNativeVersionAfter': () => nativeVersionIsSupported,
        },
        dispatch: jest.fn(),
        state: {
          device: {
            isNativeApp: true,
          },
        },
      };
    });

    describe('Supported native version', () => {
      describe('Navigation', () => {
        beforeEach(() => {
          callsNotificationsPrompt({});
        });

        it('will call next', () => {
          expect(next).toBeCalled();
        });
      });
    });

    describe('Unsupported native version', () => {
      describe('Navigation', () => {
        beforeEach(() => {
          nativeVersionIsSupported = false;
          callsNotificationsPrompt({});
        });

        it('will call next with index', () => {
          expect(next).toBeCalledWith({
            name: INDEX_NAME,
            params: {},
            query: undefined,
          });
        });
      });
    });
  });

  describe('Not on native platform', () => {
    beforeEach(() => {
      next = jest.fn();
      store = {
        getters: {
          'session/isLoggedIn': jest.fn().mockReturnValue(true),
        },
        state: {
          device: {
            isNativeApp: false,
          },
        },
      };
    });

    describe('Navigation', () => {
      beforeEach(() => {
        callsNotificationsPrompt({});
      });

      it('will call next with index', () => {
        expect(next).toBeCalledWith({
          name: INDEX_NAME,
          params: {},
          query: undefined,
        });
      });
    });
  });
});
