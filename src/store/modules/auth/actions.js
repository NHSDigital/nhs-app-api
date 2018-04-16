import Vue from 'vue';
import {
  AUTH_RESPONSE,
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

export default {
  handleAuthResponse,
};
