import NativeAppCallbacksPlugin from '@/plugins/native-app-callbacks';
import store from '@/store';

jest.mock('@/store');

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

  describe('appVersionUpdateNativeVersion', () => {
    it('will call store appVersion/updateNativeVersion', () => {
      const payload = {};
      window.nativeAppCallbacks.appVersionUpdateNativeVersion(payload);
      expect(store.dispatch).toHaveBeenCalledWith('appVersion/updateNativeVersion', payload);
    });
  });
});
