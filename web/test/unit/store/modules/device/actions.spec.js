import actions from '@/store/modules/device/actions';
import { UPDATE_IS_NATIVE_APP } from '@/store/modules/device/mutation-types';
import NativeCallbacks from '@/services/native-app';
import each from 'jest-each';

jest.mock('@/services/native-app');

const {
  updateIsNativeApp,
  unlockNavBar,
} = actions;

describe('updateIsNativeApp', () => {
  it('will call commit with the sent value', () => {
    const newIsNativeAppValue = true;

    const commit = jest.fn();

    updateIsNativeApp({ commit }, newIsNativeAppValue);

    expect(commit).toBeCalledWith(UPDATE_IS_NATIVE_APP, newIsNativeAppValue);
  });
});

describe('unlockNavBar', () => {
  beforeEach(() => {
    NativeCallbacks.pageLoadComplete.mockClear();
  });

  each([
    true,
    false,
  ]).it('will call NativeCallbacks.pageLoadComplete if process.client is true', (onClient) => {
    process.client = onClient;
    unlockNavBar();

    expect(NativeCallbacks.pageLoadComplete).toHaveBeenCalledTimes(onClient ? 1 : 0);
  });
});
