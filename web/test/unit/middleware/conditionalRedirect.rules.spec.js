import conditionalRedirect from '@/middleware/conditionalRedirect';
import { ACCOUNT_NOTIFICATIONS, INDEX, MORE } from '@/lib/routes';

describe('middleware/conditionalRedirect rules', () => {
  let getters;
  let redirect;
  let store;

  const callConditionalRedirect = (route) => {
    conditionalRedirect({ redirect, route, store });
  };

  beforeEach(() => {
    getters = [];
    store = {
      getters,
    };
    redirect = jest.fn();
  });

  describe('notifications redirect rules', () => {
    describe('is not native app', () => {
      beforeEach(() => {
        getters['device/isNativeApp'] = false;
        callConditionalRedirect(ACCOUNT_NOTIFICATIONS);
      });

      it('will redirect to accounts', () => {
        expect(redirect).toBeCalledWith('302', INDEX.path);
      });
    });

    describe('is native app', () => {
      beforeEach(() => {
        getters['device/isNativeApp'] = true;
        callConditionalRedirect(ACCOUNT_NOTIFICATIONS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('more redirect rules', () => {
    describe('is not native app', () => {
      beforeEach(() => {
        getters['device/isNativeApp'] = false;
        callConditionalRedirect(MORE);
      });
    });

    describe('is native app', () => {
      beforeEach(() => {
        getters['device/isNativeApp'] = true;
        callConditionalRedirect(MORE);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });
});
