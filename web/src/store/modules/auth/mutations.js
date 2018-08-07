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
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.onLogout();
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
