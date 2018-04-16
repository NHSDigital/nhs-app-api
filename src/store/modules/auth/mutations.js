import {
  AUTH_RESPONSE,
} from './mutation-types';

export default {
  [AUTH_RESPONSE](state) {
    state.loggedIn = true;
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.loggedIn();
    }
  },
};
