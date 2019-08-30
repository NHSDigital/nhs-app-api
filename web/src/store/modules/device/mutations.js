/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { CHECKYOURSYMPTOMS, GP_FINDER } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, GO_TO_CHECK_SYMPTOMS, GO_TO_GP_FINDER, initialState } from './mutation-types';

export default {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [GO_TO_CHECK_SYMPTOMS]() {
    if (window.nativeApp) {
      const sourceValue = this.app.store.state.device.source;
      redirectTo(this, '/check-your-symptoms', { source: sourceValue });
    } else {
      redirectTo(this, CHECKYOURSYMPTOMS.path, null);
    }
  },
  [GO_TO_GP_FINDER](state) {
    if (window.nativeApp) {
      const sourceValue = this.app.store.state.device.source;
      redirectTo(this, '/gp-finder', { source: sourceValue });
    } else {
      redirectTo(this, GP_FINDER.path, null);
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
