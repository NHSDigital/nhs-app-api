import each from 'jest-each';
import NativeCallbacks from '@/services/native-app';

describe('NATIVE CALLBACKS', () => {
  each([
    { fn: 'onLogin' },
    { fn: 'onLogout' },
    { fn: 'clearMenuBarItem' },
    { fn: 'hideHeader' },
    { fn: 'showHeader' },
    { fn: 'checkSymptoms' },
    { fn: 'hideHeaderSlim' },
    { fn: 'showHeaderSlim' },
    { fn: 'hideWhiteScreen' },
    { fn: 'completeAppIntro' },
    { fn: 'resetPageFocus' },
    { fn: 'goToLoginOptions' },
    { fn: 'fetchNativeAppVersion' },
    { fn: 'attemptBiometricLogin' },
    {
      fn: 'updateHeaderText',
      param: 'headerText',
    },
    {
      fn: 'postNdopToken',
      param: 'token',
    },
    { fn: 'onSessionExpiring' },
    { fn: 'pageLoadComplete' },
  ]).it('will trigger native callback via window.nativeApp', ({ fn, param }) => {
    const mockFunction = jest.fn();
    window.nativeApp = {
      [fn]: mockFunction,
    };
    NativeCallbacks[fn](param);

    if (param) expect(mockFunction).toHaveBeenCalledWith(param);
    else {
      setTimeout(() => {
        expect(mockFunction).toHaveBeenCalled();
      }, 20);
    }
  });
});
