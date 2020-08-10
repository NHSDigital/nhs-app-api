export default {
  attemptBiometricLogin() {
    const app = window.nativeApp;
    if (app && app.attemptBiometricLogin) {
      app.attemptBiometricLogin();
      return true;
    }
    return false;
  },

  supportsNativeWebIntegration() {
    const app = window.nativeApp;
    return app && app.openWebIntegration;
  },

  openWebIntegration(url) {
    const app = window.nativeApp;
    const request = JSON.stringify({ url });
    app.openWebIntegration(request);
  },

  clearMenuBarItem() {
    const app = window.nativeApp;
    if (app && app.clearMenuBarItem) {
      app.clearMenuBarItem();
      return true;
    }
    return false;
  },

  configureWebContext(helpUrl, reloadPath) {
    const app = window.nativeApp;
    this.setHelpUrl(helpUrl); // For backwards compatibility

    if (app && app.configureWebContext) {
      app.configureWebContext(helpUrl, reloadPath);
      return true;
    }

    return false;
  },

  dismissProgressBar() {
    const app = window.nativeApp;
    if (app && app.dismissProgressBar) {
      app.dismissProgressBar();
      return true;
    }
    return false;
  },

  displayPageLeaveWarning() {
    const app = window.nativeApp;
    if (app && app.displayPageLeaveWarning) {
      app.displayPageLeaveWarning();
      return true;
    }
    return false;
  },

  fetchNativeAppVersion() {
    const app = window.nativeApp;
    if (app && app.fetchNativeAppVersion) {
      const appVersion = app.fetchNativeAppVersion();
      return appVersion;
    }
    return '';
  },

  fetchBiometricSpec() {
    const app = window.nativeApp;
    if (app && app.fetchBiometricSpec) {
      const biometricSpec = app.fetchBiometricSpec();
      return biometricSpec;
    }
    return '';
  },

  getNotificationsStatus() {
    const app = window.nativeApp;
    if (app && app.getNotificationsStatus) {
      app.getNotificationsStatus();
      return true;
    }
    return false;
  },

  goToLoginOptions() {
    const app = window.nativeApp;
    if (app && app.goToLoginOptions) {
      app.goToLoginOptions();
      return true;
    }
    return false;
  },

  goToLoginOptionsExists() {
    const app = window.nativeApp;
    if (app && app.goToLoginOptions) {
      return true;
    }
    return false;
  },

  hideElements() {
    this.hideHeader();
    this.hideHeaderSlim();
    this.hideMenuBar();
    this.hideWhiteScreen();
  },

  hideHeader() {
    const app = window.nativeApp;
    if (app && app.hideHeader) {
      app.hideHeader();
      return true;
    }
    return false;
  },

  hideHeaders() {
    this.hideHeader();
    this.hideHeaderSlim();
  },

  hideHeaderSlim() {
    const app = window.nativeApp;
    if (app && app.hideHeaderSlim) {
      app.hideHeaderSlim();
      return true;
    }
    return false;
  },

  hideMenuBar() {
    const app = window.nativeApp;
    if (app && app.hideMenuBar) {
      app.hideMenuBar();
      return true;
    }
    return false;
  },

  hideWhiteScreen() {
    const app = window.nativeApp;
    if (app && app.hideWhiteScreen) {
      app.hideWhiteScreen();
      return true;
    }
    return false;
  },

  onLogin() {
    const app = window.nativeApp;
    if (app && app.onLogin) {
      app.onLogin();
      return true;
    }
    return false;
  },

  onLogout() {
    const app = window.nativeApp;
    if (app && app.onLogout) {
      app.onLogout();
      return true;
    }
    return false;
  },

  onSessionExpiring() {
    const app = window.nativeApp;
    if (app && app.onSessionExpiring) {
      app.onSessionExpiring();
      return true;
    }
    return false;
  },

  openAppSettings() {
    const app = window.nativeApp;
    if (app && app.openAppSettings) {
      app.openAppSettings();
      return true;
    }
    return false;
  },

  pageLoadComplete() {
    const app = window.nativeApp;
    if (app && app.pageLoadComplete) {
      app.pageLoadComplete();
      return true;
    }
    return false;
  },

  requestPnsToken(trigger) {
    const app = window.nativeApp;
    if (app && app.requestPnsToken) {
      app.requestPnsToken(trigger);
      return true;
    }
    return false;
  },

  resetPageFocus() {
    const app = window.nativeApp;
    if (app && app.resetPageFocus) {
      app.resetPageFocus();
      return true;
    }
    return false;
  },

  /**
   * @deprecated since version 1.21, here for backwards compatibility
   * */
  setHelpUrl(url) {
    const app = window.nativeApp;
    if (app && app.setHelpUrl) {
      app.setHelpUrl(url);
      return true;
    }
    return false;
  },

  /**
   * Android only - for document zoom
   * @param {boolean} canZoom
   */
  setZoomable(canZoom) {
    const app = window.nativeApp;
    if (app && app.setZoomable) {
      app.setZoomable(canZoom);
      return true;
    }
    return false;
  },

  setMenuBarItem(index) {
    const app = window.nativeApp;
    if (app && app.setMenuBarItem) {
      app.setMenuBarItem(index);
      return true;
    }
    return false;
  },

  showHeader() {
    const app = window.nativeApp;
    if (app && app.showHeader) {
      app.showHeader();
      return true;
    }
    return false;
  },

  showHeaderSlim() {
    const app = window.nativeApp;
    if (app && app.showHeaderSlim) {
      app.showHeaderSlim();
      return true;
    }
    return false;
  },

  showMenuBar() {
    const app = window.nativeApp;
    if (app && app.showMenuBar) {
      app.showMenuBar();
      return true;
    }
    return false;
  },

  startDownload(base64Data, fileName, mimeType) {
    const app = window.nativeApp;
    if (app && app.startDownload) {
      app.startDownload(base64Data, fileName, mimeType);
      return true;
    }
    return false;
  },

  addEventToCalendar(calendarEvent) {
    const app = window.nativeApp;
    if (app && app.addEventToCalendar) {
      app.addEventToCalendar(calendarEvent);
      return true;
    }
    return false;
  },

  updateBiometricRegistrationWithToken(accessToken) {
    const app = window.nativeApp;
    if (app && app.updateBiometricRegistrationWithToken) {
      app.updateBiometricRegistrationWithToken(accessToken);
      return true;
    }

    // NHSO-10729: Remove once minimum native supported version is 1.38
    if (app && app.updateBiometricRegistration) {
      app.updateBiometricRegistration();
      return true;
    }

    return false;
  },

  dismissPageLeaveWarningDialogue: () => {
    const app = window.nativeApp;

    if (app && app.dismissPageLeaveWarningDialogue) {
      app.dismissPageLeaveWarningDialogue();
      return true;
    }

    return false;
  },

  dismissAllDialogues: () => {
    const app = window.nativeApp;

    if (app && app.dismissAllDialogues) {
      app.dismissAllDialogues();
      return true;
    }

    return false;
  },
};
