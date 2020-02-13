import { CHECKYOURSYMPTOMS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, GO_TO_CHECK_SYMPTOMS, initialState } from './mutation-types';

export default {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [GO_TO_CHECK_SYMPTOMS]() {
    if (window.nativeApp) {
      const sourceValue = this.app.store.state.device.source;
      redirectTo(this, '/check-your-symptoms', { source: sourceValue });
    } else {
      redirectTo(this, CHECKYOURSYMPTOMS.path);
    }
  },
  [SET_SOURCE_DEVICE](state, source) {
    state.source = source;
  },
  [INIT_DEVICE](state) {
    const { isNativeApp, source } = state;
    // eslint-disable-next-line no-param-reassign
    state = Object.assign({}, initialState, { isNativeApp, source });
  },
};
