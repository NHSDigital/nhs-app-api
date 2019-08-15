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
  callNative('pageLoadComplete');
  callNative('requestPnsToken');
  callNative('resetPageFocus');
  callNative('setMenuBarItem', 0);
  callNative('showHeader');
  callNative('showHeaderSlim');
  callNative('storeBetaCookie');
  callNative('updateHeaderText', 'new header');

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
});
