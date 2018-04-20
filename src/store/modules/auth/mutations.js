import {
  AUTH_RESPONSE,
  LOGOUT,
  UPDATE_CONFIG,
} from './mutation-types';

export default {
  [AUTH_RESPONSE](state, user) {
    state.loggedIn = true;
    state.authorised = true;
    state.user = Object.assign({}, state.user, user);
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.loggedIn();
    }
  },
  [LOGOUT](state) {
    state.loggedIn = false;
  },
  [UPDATE_CONFIG](state, config) {
    state.config = config;
  },
};
