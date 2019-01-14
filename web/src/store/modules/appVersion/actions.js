import { INIT_APP_VERSION, UPDATE_WEB_VERSION, UPDATE_NATIVE_VERSION } from './mutation-types';

export default {
  updateWebVersion({ commit }, webVersion) {
    const versionRegex = new RegExp('[v][0-9]+');
    let correctWebVersion = webVersion;
    if (versionRegex.exec(webVersion)) {
      correctWebVersion = webVersion.substr(1);
    }
    commit(UPDATE_WEB_VERSION, correctWebVersion);
  },
  updateNativeVersion({ commit }, nativeVersion) {
    commit(UPDATE_NATIVE_VERSION, nativeVersion);
  },
  init({ commit }) {
    commit(INIT_APP_VERSION);
  },
};
