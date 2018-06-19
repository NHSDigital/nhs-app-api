import { AUTH_RESPONSE, LOGOUT, INIT_AUTH, UPDATE_CONFIG } from './mutation-types';

export default {
  [AUTH_RESPONSE](state, user) {
    state.loggedIn = true;
    state.authorised = true;
    state.user = Object.assign({}, state.user, user);
    if (window !== undefined && typeof window.nativeApp !== 'undefined') {
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
  [UPDATE_CONFIG](state, config) {
    if (state.config === {} || !state.config.codeVerifier) {
      state.config = config;
    }
  },
};

