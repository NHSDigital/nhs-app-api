import actions from '@/store/modules/device/actions';
import { UPDATE_IS_NATIVE_APP } from '@/store/modules/device/mutation-types';

const { updateIsNativeApp } = actions;
describe('updateIsNativeApp', () => {
  it('will call commit with the sent value', () => {
    const newIsNativeAppValue = true;

    const commit = jest.fn();

    updateIsNativeApp({ commit }, newIsNativeAppValue);

    expect(commit).toBeCalledWith(UPDATE_IS_NATIVE_APP, newIsNativeAppValue);
  });
});
