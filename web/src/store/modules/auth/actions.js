import AuthorisationService from '../../../services/authorization-service';
import { AUTH_RESPONSE, LOGOUT, INIT_AUTH, UPDATE_CONFIG } from './mutation-types';

export default {
  handleAuthResponse({ commit, state }, { code }) {
    /**
     * This needs to fire a proxy method
     * as more work needs to be done before logging in
     * for now we will just edit the state object.
     */
    return this.app.$http
      .postV1Session({
        userSession: {
          authCode: code,
          codeVerifier: state.config.codeVerifier,
        },
      })
      .then((response) => {
        this.dispatch('session/setDurationSeconds', response.sessionTimeout);
        this.dispatch('session/hideExpiryMessage');
        commit(AUTH_RESPONSE, response);
        this.dispatch('session/startValidationChecking');
        this.app.router.push({
          name: 'index',
        });
      });
  },
  logoutWhenExpired() {
    this.dispatch('session/showExpiryMessage');
    this.dispatch('auth/logout');
  },
  logout({ commit }) {
    this.dispatch('session/clear');
    this.dispatch('session/endValidationChecking');
    this.dispatch('errors/disableApiError');

    const final = () => {
      commit(LOGOUT, true);
      this.dispatch('availableAppointments/init');
      this.dispatch('myAppointments/init');
      this.dispatch('auth/init');
      this.dispatch('device/init');
      this.dispatch('header/init');
      this.dispatch('http/init');
      this.dispatch('navigation/init');
      this.dispatch('prescriptions/init');
      this.dispatch('repeatPrescriptionCourses/init');
      this.dispatch('errors/clearAllApiErrors');
      this.dispatch('flashMessage/init');
      this.app.router.push('/login');
    };

    return this.app.$http.deleteV1Session().then(final).catch(final);
  },
  init({ commit }) {
    commit(INIT_AUTH);
  },
  buildLogin({ commit }) {
    const codeVerifier = AuthorisationService.createVerifier();
    const loginObj = new AuthorisationService().buildLoginObject(codeVerifier);
    loginObj.codeVerifier = codeVerifier;
    commit(UPDATE_CONFIG, loginObj);
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
  updateConfig({ commit }, config) {
    commit(UPDATE_CONFIG, config);
  },
  unauthorised({ commit }) {
    commit(LOGOUT, true);
    this.dispatch('availableAppointments/init');
    this.dispatch('auth/init');
    this.dispatch('device/init');
    this.dispatch('header/init');
    this.dispatch('http/init');
    this.dispatch('navigation/init');
    this.dispatch('prescriptions/init');
    this.dispatch('repeatPrescriptionCourses/init');
    this.dispatch('flashMessage/init');
    this.app.router.push('/login');
  },
};
