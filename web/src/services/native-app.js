export default {
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

  updateHeaderText(header) {
    const app = window.nativeApp;
    if (app && app.updateHeaderText) {
      app.updateHeaderText(header);
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

  clearMenuBarItem() {
    const app = window.nativeApp;
    if (app && app.clearMenuBarItem) {
      app.clearMenuBarItem();
      return true;
    }
    return false;
  },

  hideHeader() {
    const app = window.nativeApp;
    if (app && app.hideHeader) {
      app.hideHeader();
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

  hideHeaderSlim() {
    const app = window.nativeApp;
    if (app && app.hideHeaderSlim) {
      app.hideHeaderSlim();
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

  completeAppIntro() {
    const app = window.nativeApp;
    if (app && app.completeAppIntro) {
      app.completeAppIntro();
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

  // iOS specific
  postNdopToken(token) {
    const app = window.nativeApp;
    if (app && app.postNdopToken) {
      app.postNdopToken(token);
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

  attemptBiometricLogin() {
    const app = window.nativeApp;
    if (app && app.attemptBiometricLogin) {
      app.attemptBiometricLogin();
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

  onSessionExpiring(sessionDuration) {
    const app = window.nativeApp;
    if (app && app.onSessionExpiring) {
      app.onSessionExpiring(sessionDuration);
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

  pageLoadComplete() {
    const app = window.nativeApp;
    if (app && app.pageLoadComplete) {
      app.pageLoadComplete();
      return true;
    }
    return false;
  },
};
