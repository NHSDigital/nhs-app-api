import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
} from './mutation-types';

const setCookie = ({ key, value, cookies, isSecure }) => {
  if (!cookies) return;

  const cleaned = value === '' ? undefined : value;

  if (cleaned) {
    cookies.set(key, cleaned, { secure: isSecure });
  } else {
    cookies.remove(key);
  }
};

let intervalId;

export default {
  clear:
    ({ commit }) => commit(CLEAR),
  hideExpiryMessage:
    ({ commit }) => commit(HIDE_EXPIRY_MESSAGE),
  showExpiryMessage:
    ({ commit }) => commit(SHOW_EXPIRY_MESSAGE),
  updateLastCalledAt({ commit }, lastCalledAt = new Date()) {
    const session = this.app.$cookies.get('nhso.session');

    if (session && session.durationSeconds) {
      if (intervalId) {
        clearInterval(intervalId);
      }
      intervalId = setInterval(() => {
        if (process.server) return;
        this.dispatch('session/showExpiryMessage');
        this.dispatch('auth/logout', { expired: true });
        clearInterval(intervalId);
      }, session.durationSeconds * 1000);
    }

    if (session) {
      session.lastCalledAt = lastCalledAt;
      setCookie({
        key: 'nhso.session',
        value: session,
        cookies: this.app.$cookies,
        isSecure: this.app.$env.SECURE_COOKIES,
      });
    }

    commit(SET_LAST_CALLED_AT, lastCalledAt);
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
      isSecure: this.app.$env.SECURE_COOKIES,
    });

    commit(SET_INFO, info);
  },
  startValidationChecking() {
  },
  endValidationChecking: ({ commit, state }) => {
    clearInterval(state.validationInterval);
    commit(END_VALIDATION_CHECKING);
  },
  validate({ getters }) {
    if (getters.isValid()) return true;
    return false;
  },
};
