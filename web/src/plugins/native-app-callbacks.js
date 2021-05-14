import store from '@/store';
import router from '@/router';
import { REDIRECT_PARAMETER } from '@/router/names';
import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';
import { isBlankString, redirectTo } from '@/lib/utils';

const NativeAppCallbacksPlugin = {
  install() {
    window.nativeAppCallbacks = {
      loginSettingsBiometricCompletion(biometricCompletionDetails) {
        store.dispatch('loginSettings/biometricCompletion', biometricCompletionDetails);
      },
      biometricStatus(biometricStatus) {
        store.dispatch('loginSettings/biometricStatus', biometricStatus);
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
      navigationGoToMore() {
        store.dispatch('navigation/goToMorePage');
      },
      navigationGoToHome() {
        store.dispatch('navigation/goToHomePage');
      },
      redirectToTargetUrl(url) {
        if (url && !isBlankString(url)) {
          const encodedUri = encodeURIComponent(url);
          const path = `/${INTERSTITIAL_REDIRECTOR_PATH}`;
          const query = { [REDIRECT_PARAMETER]: encodedUri };
          redirectTo({ $router: router, $store: store }, path, query, true);
        }
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
