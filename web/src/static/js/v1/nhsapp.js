// eslint-disable-next-line import/prefer-default-export
const AppPage = {
  HOME_PAGE: 'homePage',
  APPOINTMENTS: 'appointments',
  PRESCRIPTIONS: 'prescriptions',
  HEALTH_RECORDS: 'healthRecords',
  SYMPTOMS: 'symptoms',
  MESSAGES: 'messages',
};

window.nhsapp = {
  navigation: {
    goToHomePage() {
      // TODO: refactor to use goToPage(AppPage.HOME_PAGE)
      window.nhsappNative.goToHomepage();
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
