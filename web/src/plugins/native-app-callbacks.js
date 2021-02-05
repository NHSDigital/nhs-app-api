import store from '@/store';

const NativeAppCallbacksPlugin = {
  install() {
    window.nativeAppCallbacks = {
      loginSettingsBiometricCompletion(biometricCompletionDetails) {
        store.dispatch('loginSettings/biometricCompletion', biometricCompletionDetails);
      },
      loginSettingsBiometricSpec(biometricSpecDetails) {
        store.dispatch('loginSettings/biometricSpec', biometricSpecDetails);
      },
      loginHandleBiometricLoginFailure() {
        store.dispatch('login/handleBiometricLoginFailure');
      },
      sessionExtend() {
        store.dispatch('session/extend');
      },
      pageLeaveWarningLeavePage() {
        store.dispatch('pageLeaveWarning/leavePage');
      },
      pageLeaveWarningStayOnPage() {
        store.dispatch('pageLeaveWarning/stayOnPage');
      },
      authLogout() {
        store.dispatch('auth/logout');
      },
      notificationsSettingsStatus(notificationsStatus) {
        store.dispatch('notifications/settingsStatus', notificationsStatus);
      },
      notificationsUnauthorised() {
        store.dispatch('notifications/unauthorised');
      },
      notificationsAuthorised(notificationsAuthorisedDetails) {
        store.dispatch('notifications/authorised', notificationsAuthorisedDetails);
      },
      navigationGoTo(path) {
        store.dispatch('navigation/goTo', path);
      },
      navigationGoToAdvice() {
        store.dispatch('navigation/goToAdvicePage');
      },
      navigationGoToAppointments() {
        store.dispatch('navigation/goToAppointmentsPage');
      },
      navigationGoToPrescriptions() {
        store.dispatch('navigation/goToPrescriptionsPage');
      },
      navigationGoToYourHealth() {
        store.dispatch('navigation/goToYourHealthPage');
      },
      navigationGoToMessages() {
        store.dispatch('navigation/goToMessagesPage');
      },
      navigationGoToSettings() {
        store.dispatch('navigation/goToSettingsPage');
      },
      navigationGoToHome() {
        store.dispatch('navigation/goToHomePage');
      },
      appVersionUpdateNativeVersion(versionNumber) {
        store.dispatch('appVersion/updateNativeVersion', versionNumber);
      },
      deviceNotificationPromptCookieExists(doesCookieExist) {
        store.dispatch('notifications/deviceCookieExists', doesCookieExist);
      },
    };
  },
};

export default NativeAppCallbacksPlugin;
