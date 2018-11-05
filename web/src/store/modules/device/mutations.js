/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import NativeCallbacks from '@/services/native-app';
import { CHECKYOURSYMPTOMS } from '@/lib/routes';
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, GO_TO_CHECK_SYMPTOMS, initialState } from './mutation-types';

export default {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [GO_TO_CHECK_SYMPTOMS](state) {
    if (window.nativeApp) {
      if (!NativeCallbacks.checkSymptoms()) {
        const sourceValue = this.app.store.state.device.source;
        this.$router.push({
          path: '/check-your-symptoms',
          query: { source: sourceValue },
        });
      }
    } else {
      this.$router.push(CHECKYOURSYMPTOMS.path);
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
