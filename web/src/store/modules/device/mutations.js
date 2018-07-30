/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, initialState } from './mutation-types';

export default {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [SET_SOURCE_DEVICE](state, source) {
    state.source = source;
  },
  [INIT_DEVICE](state) {
    const { source } = state;
    state = Object.assign({}, initialState, { source });
  },
};
