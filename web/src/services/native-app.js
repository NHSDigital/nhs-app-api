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

  checkSymptoms() {
    const app = window.nativeApp;
    if (app && app.checkSymptoms) {
      app.checkSymptoms();
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

  showHeader() {
    const app = window.nativeApp;
    if (app && app.showHeader) {
      app.showHeader();
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

  // iOS specific
  postNdopToken(token) {
    const app = window.nativeApp;
    if (app && app.postNdopToken) {
      app.postNdopToken(token);
      return true;
    }
    return false;
  },
};
