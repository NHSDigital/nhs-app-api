import {
  AUTH_RESPONSE,
  LOGOUT,
} from './mutation-types';

export default {
  [AUTH_RESPONSE](state) {
    state.loggedIn = true;
    state.authorised = true;
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.loggedIn();
    }
  },
  [LOGOUT](state) {
    state.loggedIn = false;
  },
};
