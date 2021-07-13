import actions from '@/store/modules/device/actions';
import { UPDATE_IS_NATIVE_APP } from '@/store/modules/device/mutation-types';
import NativeApp from '@/services/native-app';

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
    NativeApp.pageLoadComplete.mockClear();
  });

  it('will call NativeApp.pageLoadComplete', () => {
    unlockNavBar();
    expect(NativeApp.pageLoadComplete).toHaveBeenCalledTimes(1);
  });
});

describe('updateReferrer', () => {
  it('will set referrer for android', () => {
    const commit = jest.fn();
    const rootState = {
      device: {
        source: 'android',
      },
    };

    const referrerData = 'test';

    actions.updateReferrer({ commit, rootState }, referrerData);
    expect(commit).toHaveBeenCalledWith('SET_APP_REFERRER', 'test');
  });

  it('will not call to log metrics for ios', () => {
    const commit = jest.fn();
    const rootState = {
      device: {
        source: 'ios',
      },
    };

    const referrerData = {
      installReferrer: 'test',
    };
    actions.updateReferrer({ commit, rootState }, referrerData);
    expect(commit).not.toHaveBeenCalled();
  });
});
