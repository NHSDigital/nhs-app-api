import NativeCallbacks from '@/services/native-app';
import {
  AUTH_RESPONSE,
  LOGOUT,
  INIT_AUTH,
  UPDATE_CONFIG,
  SET_REDIRECT_URI,
} from './mutation-types';

export default {
  [AUTH_RESPONSE](state, user) {
    state.authorised = true;
    state.user = user;
  },
  [LOGOUT]() {
    if (process.client && window.nativeApp) {
      NativeCallbacks.onLogout();
    }
  },
  [INIT_AUTH](state) {
    state.config = {};
    state.user = {};
  },
  [SET_REDIRECT_URI](state, uri) {
    state.redirectUri = uri;
  },
  [UPDATE_CONFIG](state, config) {
    state.config = config;
  },
};
