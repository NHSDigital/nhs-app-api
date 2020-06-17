import actions from '@/store/modules/device/actions';
import { UPDATE_IS_NATIVE_APP } from '@/store/modules/device/mutation-types';
import NativeCallbacks from '@/services/native-app';

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

  it('will call NativeCallbacks.pageLoadComplete', () => {
    unlockNavBar();
    expect(NativeCallbacks.pageLoadComplete).toHaveBeenCalledTimes(1);
  });
});
