import AuthorisationService from '../../../services/authorization-service';
import { AUTH_RESPONSE, LOGOUT, INIT_AUTH, UPDATE_CONFIG } from './mutation-types';

export default {
  handleAuthResponse({ commit, state }, { code }) {
  /**
   * This needs to fire a proxy method
   * as more work needs to be done before logging in
   * for now we will just edit the state object.
   */
    this.app.$http
      .postV1Session({
        userSession: {
          authCode: code,
          codeVerifier: state.config.codeVerifier,
        },
      })
      .then((response) => {
        commit(AUTH_RESPONSE, response);
        this.app.router.push({
          name: 'index',
        });
      });
  },
  logout({ commit }) {
    this.app.$http.deleteV1Session().then(() => {
      commit(LOGOUT, true);
      this.dispatch('appointmentSlots/init');
      this.dispatch('auth/init');
      this.dispatch('device/init');
      this.dispatch('header/init');
      this.dispatch('http/init');
      this.dispatch('navigation/init');
      this.dispatch('prescriptions/init');
      this.dispatch('repeatPrescriptionCourses/init');
      this.app.router.push('/login');
    });
  },
  init({ commit }) {
    commit(INIT_AUTH);
  },
  login({ dispatch, commit }, configObj) {
    const config = Object.assign({}, configObj);
    config.codeVerifier = AuthorisationService.createVerifier();
    commit(UPDATE_CONFIG, config);
    dispatch('performLogin');
  },
  performLogin({ state }) {
    new AuthorisationService().performLogin(state.config.codeVerifier);
  },
  register({ dispatch, commit }, configObj) {
    const config = Object.assign({}, configObj);
    config.codeVerifier = AuthorisationService.createVerifier();
    commit(UPDATE_CONFIG, config);
    dispatch('performRegistration');
  },
  performRegistration({ state }) {
    new AuthorisationService().performRegistration(state.config.codeVerifier);
  },
  unauthorised({ commit }) {
    commit(LOGOUT, true);
    this.dispatch('appointmentSlots/init');
    this.dispatch('auth/init');
    this.dispatch('device/init');
    this.dispatch('header/init');
    this.dispatch('http/init');
    this.dispatch('navigation/init');
    this.dispatch('prescriptions/init');
    this.dispatch('repeatPrescriptionCourses/init');
    this.app.router.push('/login');
  },
};
