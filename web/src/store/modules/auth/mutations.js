import {
  AUTH_RESPONSE,
  LOGOUT,
  INIT_AUTH,
  UPDATE_CONFIG,
  SET_REDIRECT_URI,
} from './mutation-types';

export default {
  [AUTH_RESPONSE](state, user) {
    state.loggedIn = true;
    state.authorised = true;
    state.user = user;
    if (typeof window !== 'undefined' && typeof window.nativeApp !== 'undefined') {
      window.nativeApp.onLogin();
    }
  },
  [LOGOUT](state) {
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.onLogout();
    }

    state.loggedIn = false;
  },
  [INIT_AUTH](state) {
    state.loggedIn = false;
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
