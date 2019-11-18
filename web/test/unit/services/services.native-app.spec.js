import nativeApp from '@/services/native-app';

describe('native app', () => {
  const callNative = (name, arg) => {
    describe(name, () => {
      describe('native method exists', () => {
        let result;

        beforeEach(() => {
          global.nativeApp[name] = jest.fn();
          result = nativeApp[name](arg);
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
          result = nativeApp[name](arg);
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

  callNative('attemptBiometricLogin');
  callNative('clearMenuBarItem');
  callNative('goToLoginOptions');
  callNative('hideHeader');
  callNative('hideHeaderSlim');
  callNative('hideMenuBar');
  callNative('hideWhiteScreen');
  callNative('onLogin');
  callNative('onLogout');
  callNative('onSessionExpiring', 115);
  callNative('openAppSettings');
  callNative('pageLoadComplete');
  callNative('requestPnsToken', 'load');
  callNative('resetPageFocus');
  callNative('setMenuBarItem', 0);
  callNative('setHelpUrl', 'helpUrl');
  callNative('showHeader');
  callNative('showHeaderSlim');
  callNative('storeBetaCookie');
  callNative('updateHeaderText', 'new header');

  describe('configureWebContext', () => {
    let result;
    let setHelpUrl;

    describe('native method exists', () => {
      const helpUrl = 'helpUrl';
      const reloadPath = 'reloadPath';

      beforeEach(() => {
        global.nativeApp.configureWebContext = jest.fn();
        setHelpUrl = jest.spyOn(nativeApp, 'setHelpUrl');
        result = nativeApp.configureWebContext(helpUrl, reloadPath);
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
        setHelpUrl = jest.spyOn(nativeApp, 'setHelpUrl');
        result = nativeApp.configureWebContext(helpUrl, reloadPath);
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
        result = nativeApp.fetchNativeAppVersion();
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
        result = nativeApp.fetchNativeAppVersion();
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
      hideHeader = jest.spyOn(nativeApp, 'hideHeader');
      hideHeaderSlim = jest.spyOn(nativeApp, 'hideHeaderSlim');
      hideMenuBar = jest.spyOn(nativeApp, 'hideMenuBar');
      hideWhiteScreen = jest.spyOn(nativeApp, 'hideWhiteScreen');

      nativeApp.hideElements();
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
      hideHeader = jest.spyOn(nativeApp, 'hideHeader');
      hideHeaderSlim = jest.spyOn(nativeApp, 'hideHeaderSlim');

      nativeApp.hideHeaders();
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
        expect(nativeApp.goToLoginOptions()).toBe(true);
      });
    });

    describe('native method does not exist', () => {
      it('will return false', () => {
        expect(nativeApp.goToLoginOptions()).toBe(false);
      });
    });
  });

  describe('startDownload', () => {
    describe('native method exists', () => {
      let startDownload;
      beforeEach(() => {
        global.nativeApp.startDownload = jest.fn();
        startDownload = jest.spyOn(global.nativeApp, 'startDownload');
        nativeApp.startDownload('base64', 'example.jpg', 'image/jpg');
      });

      it('will return true', () => {
        expect(nativeApp.startDownload()).toBe(true);
      });

      it('will call the method', () => {
        expect(startDownload).toBeCalledWith('base64', 'example.jpg', 'image/jpg');
      });
    });

    describe('native method does not exist', () => {
      it('will return false', () => {
        expect(nativeApp.startDownload()).toBe(false);
      });
    });
  });
});
