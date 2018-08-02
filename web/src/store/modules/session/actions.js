import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_DURATION_SECONDS,
  SET_GP_ODS_CODE,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SET_CSRF_TOKEN,
} from './mutation-types';

export default {
  clear:
    ({ commit }) => commit(CLEAR),
  hideExpiryMessage:
    ({ commit }) => commit(HIDE_EXPIRY_MESSAGE),
  setDurationSeconds:
    ({ commit }, numberOfSeconds) => commit(SET_DURATION_SECONDS, numberOfSeconds),
  setGpOdsCode:
    ({ commit }, odsCode) => commit(SET_GP_ODS_CODE, odsCode),
  showExpiryMessage:
    ({ commit }) => commit(SHOW_EXPIRY_MESSAGE),
  updateLastCalledAt:
    ({ commit }, lastCalledAt) => commit(SET_LAST_CALLED_AT, lastCalledAt || new Date()),
  setCsrfToken:
    ({ commit }, token) => commit(SET_CSRF_TOKEN, token),
  startValidationChecking: ({
    commit, dispatch, state, rootState,
  }) => {
    if (!rootState.auth.loggedIn) {
      return;
    }

    if (state.validationInterval) {
      return;
    }

    if (process.client) {
      const interval = setInterval(() => {
        dispatch('validate');
      }, 10000);

      commit(START_VALIDATION_CHECKING, interval);
    }
  },
  endValidationChecking: ({ commit, state }) => {
    clearInterval(state.validationInterval);
    commit(END_VALIDATION_CHECKING);
  },
  validate({ getters }) {
    if (getters.isValid()) return true;
    this.dispatch('session/endValidationChecking');
    this.dispatch('auth/logoutWhenExpired');
    return false;
  },
};
