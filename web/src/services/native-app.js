export default {
  areNotificationsEnabled() {
    const app = window.nativeApp;
    if (app && app.areNotificationsEnabled) {
      app.areNotificationsEnabled();
      return true;
    }
    return false;
  },

  attemptBiometricLogin() {
    const app = window.nativeApp;
    if (app && app.attemptBiometricLogin) {
      app.attemptBiometricLogin();
      return true;
    }
    return false;
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
    this.setHelpUrl(helpUrl); // For backwards compatability

    if (app && app.configureWebContext) {
      app.configureWebContext(helpUrl, reloadPath);
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

  onSessionExpiring(sessionDuration) {
    const app = window.nativeApp;
    if (app && app.onSessionExpiring) {
      app.onSessionExpiring(sessionDuration);
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


  /* Android only - for document zoom */
  setZoomable(canZoom) {
    const app = window.nativeApp;
    if (app && app.setZoomable) {
      app.setZoomable(canZoom);
      return true;
    }
    return false;
  },

  /**
   * @deprecated since version 1.21, here for backwards compatability
   * */
  setHelpUrl(url) {
    const app = window.nativeApp;
    if (app && app.setHelpUrl) {
      app.setHelpUrl(url);
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

  storeBetaCookie() {
    const app = window.nativeApp;
    if (app && app.storeBetaCookie) {
      app.storeBetaCookie();
      return true;
    }
    return false;
  },

  updateHeaderText(header) {
    const app = window.nativeApp;
    if (app && app.updateHeaderText) {
      app.updateHeaderText(header);
      return true;
    }
    return false;
  },
};
