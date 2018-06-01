import {
  CLEAR,
  HIDE_EXPIRY_MESSAGE,
  SET_DURATION_SECONDS,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
} from './mutation-types';

export default {
  clear: ({ commit }) => commit(CLEAR),
  hideExpiryMessage: ({ commit }) => commit(HIDE_EXPIRY_MESSAGE),
  setDurationSeconds:
    ({ commit }, numberOfSeconds) => commit(SET_DURATION_SECONDS, numberOfSeconds),
  showExpiryMessage: ({ commit }) => commit(SHOW_EXPIRY_MESSAGE),
  updateLastCalledAt:
    ({ commit }, lastCalledAt) => commit(SET_LAST_CALLED_AT, lastCalledAt || new Date()),
  validate({ getters }) {
    if (getters.isValid()) return true;
    this.dispatch('auth/logout');
    return false;
  },
};
