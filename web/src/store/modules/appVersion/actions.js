import { INIT_APP_VERSION, UPDATE_WEB_VERSION, UPDATE_NATIVE_VERSION } from './mutation-types';

export default {
  updateWebVersion({ commit }, webVersion) {
    commit(UPDATE_WEB_VERSION, webVersion);
  },
  updateNativeVersion({ commit }, nativeVersion) {
    commit(UPDATE_NATIVE_VERSION, nativeVersion);
  },
  init({ commit }) {
    commit(INIT_APP_VERSION);
  },
};
