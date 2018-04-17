import Vue from 'vue';
import {
  AUTH_RESPONSE,
  LOGOUT,
} from './mutation-types';

export const handleAuthResponse = ({ commit }) => {
  /**
   * This needs to fire a proxy method
   * as more work needs to be done before logging in
   * for now we will just edit the state object.
   */

  commit(AUTH_RESPONSE, true);
  Vue.router.push({
    name: 'home.index',
  });
};

export const unauthorised = ({ commit }) => {
  commit(LOGOUT, true);
  Vue.router.push({
    name: 'home.logout',
    params: { unauthorised: true },
  });
};

export const logout = ({ commit }) => {
  commit(LOGOUT, true);
  Vue.router.push({
    name: 'home.logout',
  });
};

export default {
  handleAuthResponse,
  unauthorised,
  logout,
};
