import { INIT_APP_VERSION, UPDATE_WEB_VERSION, UPDATE_NATIVE_VERSION, UPDATE_PLATFORM } from './mutation-types';

export default {
  updateWebVersion({ commit }, webVersion) {
    commit(UPDATE_WEB_VERSION, webVersion);
  },
  updateNativeVersion({ commit }, nativeVersion) {
    commit(UPDATE_NATIVE_VERSION, nativeVersion);
  },
  updatePlatform({ commit }, platform) {
    commit(UPDATE_PLATFORM, platform);
  },
  init({ commit }) {
    commit(INIT_APP_VERSION);
  },
};
