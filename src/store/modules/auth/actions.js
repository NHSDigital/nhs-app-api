import Vue from 'vue';
import AuthorisationService from '../../../services/authorization-service';
import {
  AUTH_RESPONSE,
  LOGOUT,
  UPDATE_CONFIG,
} from './mutation-types';

export const handleAuthResponse = ({ commit, state }, { code }) => {
  /**
   * This needs to fire a proxy method
   * as more work needs to be done before logging in
   * for now we will just edit the state object.
   */

  Vue.$http.postV1Session({
    userSession: {
      authCode: code,
      codeVerifier: state.config.codeVerifier,
    },
  }).then((response) => {
    commit(AUTH_RESPONSE, response);
    Vue.router.push({
      name: 'home.index',
    });
  });
};

export const unauthorised = ({ commit }) => {
  commit(LOGOUT, true);
  Vue.router.push({
    name: 'home.logout',
    params: { unauthorised: true },
  });
};

export const performLogin = ({ state }) => {
  new AuthorisationService(state.config).login(state.config.codeVerifier);
};

export const login = ({ dispatch, commit }, configObj) => {
  const config = Object.assign({}, configObj);
  config.codeVerifier = AuthorisationService.createVerifier();
  commit(UPDATE_CONFIG, config);
  dispatch('performLogin');
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
  login,
  performLogin,
};
