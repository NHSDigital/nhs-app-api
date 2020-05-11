window.nhsapp = window.nhsapp || {};
window.nhsapp.tools = window.nhsapp.tools || {};
window.nhsapp.navigation = window.nhsapp.navigation || {};

window.nhsapp.tools.isOpenInNHSApp = () => (navigator.userAgent.indexOf('nhsapp') !== -1);

window.nhsapp.tools.getAppPlatform = () => {
  if (navigator.userAgent.indexOf('nhsapp-android') !== -1) {
    return 'android';
  } if (navigator.userAgent.indexOf('nhsapp-ios') !== -1) {
    return 'ios';
  }
  return 'none';
};

window.nhsapp.navigation.goToHomePage = () => {
  window.nhsappNative.goToHomepage();
};
