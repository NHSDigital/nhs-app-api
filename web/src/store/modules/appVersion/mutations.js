/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { INIT_APP_VERSION, UPDATE_WEB_VERSION, UPDATE_NATIVE_VERSION, UPDATE_PLATFORM } from './mutation-types';

export default {
  [UPDATE_WEB_VERSION](state, webVersion) {
    state.webVersion = webVersion;
  },
  [UPDATE_NATIVE_VERSION](state, nativeVersion) {
    state.nativeVersion = nativeVersion;
  },
  [UPDATE_PLATFORM](state, platform) {
    state.platform = platform;
  },
  [INIT_APP_VERSION](state) {
    state = Object.assign({}, {
      webVersion: '',
      nativeVersion: '',
      platform: 'web',
    });
  },
};
