/* eslint-disable import/extensions */
import mutations from '../../../../../src/store/modules/device/mutations';
import { UPDATE_IS_NATIVE_APP } from '../../../../../src/store/modules/device/mutation-types';

describe('UPDATE_IS_NATIVE_APP', () => {
  it('will set the isNativeApp on the state to the sent value', () => {
    const state = {};
    const newIsNativeAppValue = true;

    mutations[UPDATE_IS_NATIVE_APP](state, newIsNativeAppValue);

    expect(state.isNativeApp).toEqual(newIsNativeAppValue);
  });
});
