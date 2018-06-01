import AuthorisationService from '../services/authorization-service';

const initialState = {
  loggedIn: false,
  config: {},
  user: {},
};

export const state = () => initialState;

const AUTH_RESPONSE = 'AUTH_RESPONSE';
const LOGOUT = 'LOGOUT';
const UPDATE_CONFIG = 'UPDATE_VERIFIER';
const INIT_AUTH = 'INIT_AUTH';
/* eslint-disable no-shadow */
export const actions = {
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
  logout({ commit, dispatch }) {
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
/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [AUTH_RESPONSE](state, user) {
    state.loggedIn = true;
    state.authorised = true;
    state.user = Object.assign({}, state.user, user);
    if (typeof window.nativeApp !== 'undefined') {
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
    state.config = config;
  },
};
