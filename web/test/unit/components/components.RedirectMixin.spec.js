import RedirectMixin from '@/components/RedirectMixin';
import NativeApp from '@/services/native-app';
import { redirectTo } from '@/lib/utils';

jest.mock('@/services/native-app');
jest.mock('@/lib/utils');


describe('RedirectMixin', () => {
  const self = {
    $route: {
      query: '',
    },
  };

  beforeEach(() => {
    jest.clearAllMocks();
    NativeApp.goToLoggedInHomeScreen = jest.fn();
  });

  describe('if NativeApp.goToLoggedInHomeScreen is true', () => {
    beforeEach(() => {
      NativeApp.goToLoggedInHomeScreen = jest.fn().mockImplementation(() => true);
      RedirectMixin.methods.conditionalRedirect();
    });

    it('will not call redirectTo', () => {
      expect(NativeApp.goToLoggedInHomeScreen).toBeCalled();
      expect(redirectTo).not.toBeCalled();
    });
  });

  describe('if NativeApp.goToLoggedInHomeScreen is false', () => {
    beforeEach(() => {
      NativeApp.goToLoggedInHomeScreen = jest.fn().mockImplementation(() => false);
      RedirectMixin.methods.conditionalRedirect.apply(self);
    });

    it('will will call redirectTo', () => {
      expect(NativeApp.goToLoggedInHomeScreen).toBeCalled();
      expect(redirectTo).toBeCalled();
    });
  });
});
