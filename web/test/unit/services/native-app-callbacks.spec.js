import each from 'jest-each';
import NativeCallbacks from '@/services/native-app';

describe('NATIVE CALLBACKS', () => {
  each([
    { fn: 'onLogin' },
    { fn: 'onLogout' },
    { fn: 'clearMenuBarItem' },
    { fn: 'checkSymptoms' },
    { fn: 'hideHeader' },
    { fn: 'showHeader' },
    { fn: 'hideWhiteScreen' },
    { fn: 'completeAppIntro' },
    {
      fn: 'updateHeaderText',
      param: 'headerText',
    },
    {
      fn: 'postNdopToken',
      param: 'token',
    },
  ]).it('will call the native app handle to update the page header', ({ fn, param }) => {
    const mockFunction = jest.fn();
    window.nativeApp = {
      [fn]: mockFunction,
    };
    NativeCallbacks[fn](param);

    if (param) expect(mockFunction).toHaveBeenCalledWith(param);
    else expect(mockFunction).toHaveBeenCalled();
  });
});
