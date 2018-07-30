/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, initialState } from './mutation-types';

export default {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [INIT_DEVICE](state) {
    state = initialState;
  },
};
