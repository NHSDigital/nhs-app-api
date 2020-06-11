import { AppPage } from './constants';

window.nhsapp = {
  navigation: {
    goToHomePage() {
      // TODO: remove when min app version is 1.36
      if (window.nhsappNative.goToHomepage) {
        window.nhsappNative.goToHomepage();
      } else {
        this.goToPage(AppPage.HOME_PAGE);
      }
    },
    goToPage(appPage) {
      window.nhsappNative.goToPage(appPage);
    },
  },
  tools: {
    getAppPlatform() {
      if (navigator.userAgent.indexOf('nhsapp-android') !== -1) {
        return 'android';
      } if (navigator.userAgent.indexOf('nhsapp-ios') !== -1) {
        return 'ios';
      }
      return 'none';
    },
    isOpenInNHSApp: () => navigator.userAgent.indexOf('nhsapp') !== -1,
  },
};

window.nhsapp.navigation.AppPage = AppPage;
