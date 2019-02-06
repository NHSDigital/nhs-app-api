import NativeCallbacks from '@/services/native-app';
import { setCookie } from '@/lib/cookie-manager';
import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SHOW_SESSION_EXPIRING,
  HIDE_SESSION_EXPIRING,
} from './mutation-types';


export default {
  clear:
    ({ commit }) => commit(CLEAR),
  hideExpiryMessage:
    ({ commit }) => commit(HIDE_EXPIRY_MESSAGE),
  showExpiryMessage:
    ({ commit }) => commit(SHOW_EXPIRY_MESSAGE),
  updateLastCalledAt({ commit }, lastCalledAt = new Date()) {
    const session = this.app.$cookies.get('nhso.session');

    if (session) {
      session.lastCalledAt = lastCalledAt;
      setCookie({
        key: 'nhso.session',
        value: session,
        cookies: this.app.$cookies,
        options: {
          secure: this.app.$env.SECURE_COOKIES,
        },
      });
    }

    commit(SET_LAST_CALLED_AT, lastCalledAt);
    commit(HIDE_SESSION_EXPIRING);
  },
  setInfo({ commit }, info) {
    const value = !info
      ? undefined
      : ({
        name: info.name,
        durationSeconds: info.durationSeconds,
        gpOdsCode: info.gpOdsCode,
        token: info.token,
        lastCalledAt: info.lastCalledAt || new Date(),
        nhsNumber: info.nhsNumber,
        dateOfBirth: info.dateOfBirth,
        accessToken: info.accessToken,
      });

    setCookie({
      key: 'nhso.session',
      value,
      cookies: this.app.$cookies,
      options: {
        secure: this.app.$env.SECURE_COOKIES,
      },
    });

    commit(SET_INFO, info);
  },
  startValidationChecking({ getters, commit, dispatch, state }) {
    if (process.server || !getters.isLoggedIn() || state.validationInterval) return;

    const interval = setInterval(() => {
      dispatch('validate');
    }, 5000);

    commit(START_VALIDATION_CHECKING, interval);
  },
  endValidationChecking: ({ commit, state }) => {
    clearInterval(state.validationInterval);
    commit(END_VALIDATION_CHECKING);
    commit(HIDE_SESSION_EXPIRING);
  },
  validate({ getters, state, commit }) {
    if (getters.isLoggedIn()) {
      if (getters.isValid()) {
        if (process.client && !state.showSessionExpiring && window.nativeApp
            && getters.isExpiring(this.app.$env.SESSION_EXPIRING_WARNING_SECONDS)) {
          commit(SHOW_SESSION_EXPIRING);
          NativeCallbacks.onSessionExpiring(state.durationSeconds / 60);
        }
        return true;
      }
      this.dispatch('auth/logoutWhenExpired');
    }
    return false;
  },
  extend({ dispatch }) {
    return this.app.$http.postV1SessionExtend()
      .catch(() => dispatch('auth/logoutWhenExpired'));
  },
};
