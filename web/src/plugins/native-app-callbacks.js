import store from '@/store';
import router from '@/router';
import { REDIRECT_PAGE_PARAMETER, REDIRECT_PARAMETER } from '@/router/names';
import { INTERSTITIAL_REDIRECTOR_PATH, GP_SESSION_ON_DEMAND_RETURN_PATH } from '@/router/paths';
import { generateContextualHelpLink, isBlankString, redirectTo } from '@/lib/utils';
import NativeApp from '@/services/native-app';

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
      validateSession() {
        store.dispatch('session/validate', true);
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
      navigationGoToByRouteName(name) {
        store.dispatch('navigation/goToRouteByName', name);
      },
      retrieveLastCrumbName() {
        const crumbExists = window.vue.$route.meta.crumb.defaultCrumb;
        if (crumbExists !== undefined) {
          return window.vue.$route.meta.crumb.defaultCrumb[crumbExists.length - 1].name;
        }

        return '';
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
      navigateToOnDemandGpReturn(parameters) {
        redirectTo(
          { $router: router, $store: store },
          GP_SESSION_ON_DEMAND_RETURN_PATH,
          JSON.parse(parameters),
          true,
        );
      },
      redirectToTargetUrl(url) {
        if (url && !isBlankString(url)) {
          const encodedUri = encodeURIComponent(url);
          const path = `/${INTERSTITIAL_REDIRECTOR_PATH}`;
          const query = { [REDIRECT_PARAMETER]: encodedUri };
          redirectTo({ $router: router, $store: store }, path, query, true);
        }
      },
      navigateToAppPage(page) {
        const path = `/${INTERSTITIAL_REDIRECTOR_PATH}`;
        const query = { [REDIRECT_PAGE_PARAMETER]: page };
        redirectTo({ $router: router, $store: store }, path, query, true);
      },
      appVersionUpdateNativeVersion(versionNumber) {
        store.dispatch('appVersion/updateNativeVersion', versionNumber);
      },
      deviceNotificationPromptCookieExists(doesCookieExist) {
        store.dispatch('notifications/deviceCookieExists', doesCookieExist);
      },
      getContextualHelpLink() {
        NativeApp.openBrowserOverlay(generateContextualHelpLink(store, router.currentRoute));
      },
    };
  },
};

export default NativeAppCallbacksPlugin;
