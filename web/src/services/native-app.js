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

  checkSymptoms() {
    const app = window.nativeApp;
    if (app && app.checkSymptoms) {
      app.checkSymptoms();
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

  goToBiometrics() {
    const app = window.nativeApp;
    if (app && app.goToBiometrics) {
      app.goToBiometrics();
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
};
