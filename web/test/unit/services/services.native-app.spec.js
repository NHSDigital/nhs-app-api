import NativeApp from '@/services/native-app';

describe('native app', () => {
  const callNative = (name, arg) => {
    describe(name, () => {
      describe('native method exists', () => {
        let result;

        beforeEach(() => {
          global.nativeApp[name] = jest.fn();
          result = NativeApp[name](arg);
        });

        it('will call method', () => {
          if (arg) {
            expect(global.nativeApp[name]).toBeCalledWith(arg);
          } else {
            expect(global.nativeApp[name]).toBeCalled();
          }
        });

        it('will return true', () => {
          expect(result).toBe(true);
        });
      });

      describe('native method does not exist', () => {
        let result;

        beforeEach(() => {
          result = NativeApp[name](arg);
        });

        it('will return false', () => {
          expect(result).toBe(false);
        });
      });
    });
  };

  beforeEach(() => {
    global.nativeApp = {};
  });

  callNative('clearMenuBarItem');
  callNative('getNotificationsStatus');
  callNative('goToLoginOptions');
  callNative('hideHeader');
  callNative('hideHeaderSlim');
  callNative('hideMenuBar');
  callNative('hideWhiteScreen');
  callNative('onLogin');
  callNative('onLogout');
  callNative('onSessionExpiring');
  callNative('openAppSettings');
  callNative('pageLoadComplete');
  callNative('requestPnsToken', 'load');
  callNative('setMenuBarItem', 0);
  callNative('setHelpUrl', 'helpUrl');
  callNative('showHeader');
  callNative('showHeaderSlim');
  callNative('setBadgeCount', 3);

  describe('configureWebContext', () => {
    let result;
    let setHelpUrl;

    describe('native method exists', () => {
      const helpUrl = 'helpUrl';
      const reloadPath = 'reloadPath';

      beforeEach(() => {
        global.nativeApp.configureWebContext = jest.fn();
        setHelpUrl = jest.spyOn(NativeApp, 'setHelpUrl');
        result = NativeApp.configureWebContext(helpUrl, reloadPath);
      });

      it('will call method', () => {
        expect(global.nativeApp.configureWebContext).toBeCalledWith(helpUrl, reloadPath);
      });

      it('will call `setHelpUrl` with help URL', () => {
        expect(setHelpUrl).toBeCalledWith(helpUrl);
      });

      it('will return true', () => {
        expect(result).toBe(true);
      });
    });

    describe('native method does not exists', () => {
      const helpUrl = 'helpUrl';
      const reloadPath = 'reloadPath';

      beforeEach(() => {
        setHelpUrl = jest.spyOn(NativeApp, 'setHelpUrl');
        result = NativeApp.configureWebContext(helpUrl, reloadPath);
      });

      it('will call `setHelpUrl` with help URL', () => {
        expect(setHelpUrl).toBeCalledWith(helpUrl);
      });

      it('will return false', () => {
        expect(result).toBe(false);
      });
    });
  });

  describe('fetchNativeAppVersion', () => {
    describe('native method exists', () => {
      const returnValue = 'Returning';
      let result;
      let fetchNativeAppVersion;

      beforeEach(() => {
        global.nativeApp.fetchNativeAppVersion = () => returnValue;
        fetchNativeAppVersion = jest.spyOn(global.nativeApp, 'fetchNativeAppVersion');
        result = NativeApp.fetchNativeAppVersion();
      });

      it('will call method', () => {
        expect(fetchNativeAppVersion).toBeCalled();
      });

      it('will return the value', () => {
        expect(result).toBe(returnValue);
      });
    });

    describe('native method does not exist', () => {
      let result;

      beforeEach(() => {
        result = NativeApp.fetchNativeAppVersion();
      });

      it('will return empty string', () => {
        expect(result).toBe('');
      });
    });
  });

  describe('hideElements', () => {
    let hideHeader;
    let hideHeaderSlim;
    let hideMenuBar;
    let hideWhiteScreen;

    beforeEach(() => {
      hideHeader = jest.spyOn(NativeApp, 'hideHeader');
      hideHeaderSlim = jest.spyOn(NativeApp, 'hideHeaderSlim');
      hideMenuBar = jest.spyOn(NativeApp, 'hideMenuBar');
      hideWhiteScreen = jest.spyOn(NativeApp, 'hideWhiteScreen');

      NativeApp.hideElements();
    });

    it('will call `hideHeader`', () => {
      expect(hideHeader).toBeCalled();
    });

    it('will call `hideHeaderSlim`', () => {
      expect(hideHeaderSlim).toBeCalled();
    });

    it('will call `hideMenuBar`', () => {
      expect(hideMenuBar).toBeCalled();
    });

    it('will call `hideWhiteScreen`', () => {
      expect(hideWhiteScreen).toBeCalled();
    });
  });

  describe('hideHeaders', () => {
    let hideHeader;
    let hideHeaderSlim;

    beforeEach(() => {
      hideHeader = jest.spyOn(NativeApp, 'hideHeader');
      hideHeaderSlim = jest.spyOn(NativeApp, 'hideHeaderSlim');

      NativeApp.hideHeaders();
    });

    it('will call `hideHeader`', () => {
      expect(hideHeader).toBeCalled();
    });

    it('will call `hideHeaderSlim`', () => {
      expect(hideHeaderSlim).toBeCalled();
    });
  });

  describe('goToLoginOptionsExists', () => {
    describe('native method exists', () => {
      beforeEach(() => {
        global.nativeApp.goToLoginOptions = jest.fn();
      });

      it('will return true', () => {
        expect(NativeApp.goToLoginOptions()).toBe(true);
      });
    });

    describe('native method does not exist', () => {
      it('will return false', () => {
        expect(NativeApp.goToLoginOptions()).toBe(false);
      });
    });
  });

  describe('startDownload', () => {
    describe('native method exists', () => {
      let startDownload;
      beforeEach(() => {
        global.nativeApp.startDownload = jest.fn();
        startDownload = jest.spyOn(global.nativeApp, 'startDownload');
        NativeApp.startDownload('base64', 'example.jpg', 'image/jpg');
      });

      it('will return true', () => {
        expect(NativeApp.startDownload()).toBe(true);
      });

      it('will call the method', () => {
        expect(startDownload).toBeCalledWith('base64', 'example.jpg', 'image/jpg');
      });
    });

    describe('native method does not exist', () => {
      it('will return false', () => {
        expect(NativeApp.startDownload()).toBe(false);
      });
    });
  });

  describe('updateBiometricRegistrationWithToken', () => {
    describe('native methods exists', () => {
      let result;
      beforeEach(() => {
        global.nativeApp.updateBiometricRegistrationWithToken = jest.fn();
        global.nativeApp.updateBiometricRegistration = jest.fn();
        result = NativeApp.updateBiometricRegistrationWithToken('accessToken');
      });

      it('will return true', () => {
        expect(result).toBe(true);
      });

      it('will call native method with accessToken', () => {
        expect(global.nativeApp.updateBiometricRegistrationWithToken).toBeCalledWith('accessToken');
      });

      it('will call not call legacy native method', () => {
        expect(global.nativeApp.updateBiometricRegistration).not.toBeCalled();
      });
    });

    describe('native methods do not exist', () => {
      let result;
      beforeEach(() => {
        global.nativeApp.updateBiometricRegistrationWithToken = undefined;
        global.nativeApp.updateBiometricRegistration = undefined;
        result = NativeApp.updateBiometricRegistrationWithToken('accessToken');
      });

      it('will return false', () => {
        expect(result).toBe(false);
      });
    });

    describe('when goToLoggedInHomeScreen method exists', () => {
      beforeEach(() => {
        global.nativeApp.goToLoggedInHomeScreen = jest.fn();
      });

      it('shouldShowPreLoginHeader will return false', () => {
        expect(NativeApp.shouldShowPreLoginHeader()).toBe(false);
      });
    });

    describe('when goToLoggedInHomeScreen method does not exist', () => {
      beforeEach(() => {
        global.nativeApp.goToLoggedInHomeScreen = undefined;
      });

      it('shouldShowPreLoginHeader will return true', () => {
        expect(NativeApp.shouldShowPreLoginHeader()).toBe(true);
      });
    });
  });
});
