import NativeAppCallbacksPlugin from '@/plugins/native-app-callbacks';
import store from '@/store';
import { redirectTo } from '@/lib/utils';
import router from '@/router';

jest.mock('@/store');
jest.mock('@/lib/utils');
jest.mock('@/router');

describe('native-app-callbacks', () => {
  beforeEach(() => {
    NativeAppCallbacksPlugin.install();
  });

  describe('loginSettingsBiometricCompletion', () => {
    it('will call store loginSettings/biometricCompletion', () => {
      const payload = {};
      window.nativeAppCallbacks.loginSettingsBiometricCompletion(payload);
      expect(store.dispatch).toHaveBeenCalledWith('loginSettings/biometricCompletion', payload);
    });
  });

  describe('loginSettingsBiometricSpec', () => {
    it('will call store loginSettings/biometricSpec', () => {
      const payload = {};
      window.nativeAppCallbacks.loginSettingsBiometricSpec(payload);
      expect(store.dispatch).toHaveBeenCalledWith('loginSettings/biometricSpec', payload);
    });
  });

  describe('loginHandleBiometricLoginFailure', () => {
    it('will call store login/handleBiometricLoginFailure', () => {
      window.nativeAppCallbacks.loginHandleBiometricLoginFailure();
      expect(store.dispatch).toHaveBeenCalledWith('login/handleBiometricLoginFailure');
    });
  });

  describe('sessionExtend', () => {
    it('will call store session/extend', () => {
      window.nativeAppCallbacks.sessionExtend();
      expect(store.dispatch).toHaveBeenCalledWith('session/extend');
    });
  });

  describe('pageLeaveWarningLeavePage', () => {
    it('will call store pageLeaveWarning/leavePage', () => {
      window.nativeAppCallbacks.pageLeaveWarningLeavePage();
      expect(store.dispatch).toHaveBeenCalledWith('pageLeaveWarning/leavePage');
    });
  });

  describe('pageLeaveWarningStayOnPage', () => {
    it('will call store pageLeaveWarning/stayOnPage', () => {
      window.nativeAppCallbacks.pageLeaveWarningStayOnPage();
      expect(store.dispatch).toHaveBeenCalledWith('pageLeaveWarning/stayOnPage');
    });
  });

  describe('authLogout', () => {
    it('will call store auth/logout', () => {
      window.nativeAppCallbacks.authLogout();
      expect(store.dispatch).toHaveBeenCalledWith('auth/logout');
    });
  });

  describe('notificationsSettingsStatus', () => {
    it('will call store notifications/settingsStatus', () => {
      const payload = {};
      window.nativeAppCallbacks.notificationsSettingsStatus(payload);
      expect(store.dispatch).toHaveBeenCalledWith('notifications/settingsStatus', payload);
    });
  });

  describe('notificationsUnauthorised', () => {
    it('will call store notifications/unauthorised', () => {
      window.nativeAppCallbacks.notificationsUnauthorised();
      expect(store.dispatch).toHaveBeenCalledWith('notifications/unauthorised');
    });
  });

  describe('notificationsAuthorised', () => {
    it('will call store notifications/authorised', () => {
      const payload = {};
      window.nativeAppCallbacks.notificationsAuthorised(payload);
      expect(store.dispatch).toHaveBeenCalledWith('notifications/authorised', payload);
    });
  });

  describe('navigationGoTo', () => {
    it('will call store navigation/goTo', () => {
      const payload = {};
      window.nativeAppCallbacks.navigationGoTo(payload);
      expect(store.dispatch).toHaveBeenCalledWith('navigation/goTo', payload);
    });
  });

  describe('redirectToTargetUrl', () => {
    beforeEach(() => {
      redirectTo.mockClear();
    });
    describe('targetUrl is populated', () => {
      beforeEach(() => {
        window.nativeAppCallbacks.redirectToTargetUrl('http://www.mock.url');
      });

      it('will call redirectTo with http://www.mock.url', () => {
        expect(redirectTo).toHaveBeenCalledWith({ $router: router, $store: store }, '/redirector', { redirect_to: 'http%3A%2F%2Fwww.mock.url' }, true);
      });
    });

    describe('targetUrl is undefined', () => {
      beforeEach(() => {
        window.nativeAppCallbacks.redirectToTargetUrl(undefined);
      });

      it('will not call redirectTo', () => {
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });

    describe('targetUrl is blank', () => {
      beforeEach(() => {
        window.nativeAppCallbacks.redirectToTargetUrl('');
      });

      it('will not call redirectTo', () => {
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });
  });

  describe('appVersionUpdateNativeVersion', () => {
    it('will call store appVersion/updateNativeVersion', () => {
      const payload = {};
      window.nativeAppCallbacks.appVersionUpdateNativeVersion(payload);
      expect(store.dispatch).toHaveBeenCalledWith('appVersion/updateNativeVersion', payload);
    });
  });

  describe('deviceNotificationPromptCookieExists', () => {
    it('will call store notifications/deviceCookieExists', () => {
      window.nativeAppCallbacks.deviceNotificationPromptCookieExists(true);

      expect(store.dispatch).toHaveBeenCalledWith('notifications/deviceCookieExists', true);
    });
  });
});
