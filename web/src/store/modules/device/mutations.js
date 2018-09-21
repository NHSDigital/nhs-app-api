/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, GO_TO_CHECK_SYMPTOMS, initialState } from './mutation-types';

export default {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [GO_TO_CHECK_SYMPTOMS](state) {
    if (state.isNativeApp === true) {
      window.nativeApp.checkSymptoms();
    } else {
      this.$router.push('/check-your-symptoms');
    }
  },
  [SET_SOURCE_DEVICE](state, source) {
    state.source = source;
  },
  [INIT_DEVICE](state) {
    const { isNativeApp, source } = state;
    state = Object.assign({}, initialState, { isNativeApp, source });
  },
};
