import conditionalRedirect from '@/middleware/conditionalRedirect';
import { MORE_ACCOUNT_AND_SETTINGS_MANAGE_NOTIFICATIONS } from '@/router/routes/more';
import { INDEX_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';

describe('middleware/conditionalRedirect rules', () => {
  let getters;
  let next;
  let store;
  dependency.createRoutePathObject = jest.fn(x => ({ path: x.path }));

  const callConditionalRedirect = (to) => {
    conditionalRedirect({ next, to, store });
  };

  beforeEach(() => {
    getters = [];
    store = {
      getters,
    };
    next = jest.fn();
  });

  describe('notifications next rules', () => {
    describe('is not native app', () => {
      beforeEach(() => {
        getters['device/isNativeApp'] = false;
        callConditionalRedirect(MORE_ACCOUNT_AND_SETTINGS_MANAGE_NOTIFICATIONS);
      });

      it('will next to accounts', () => {
        expect(next).toBeCalledWith({ path: INDEX_PATH });
      });
    });

    describe('is native app', () => {
      beforeEach(() => {
        getters['device/isNativeApp'] = true;
        callConditionalRedirect(MORE_ACCOUNT_AND_SETTINGS_MANAGE_NOTIFICATIONS);
      });

      it('will next with no parameter', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });
  });
});
