import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_DURATION_SECONDS,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SET_CSRF_TOKEN,
  SAVE_COOKIE,
} from './mutation-types';

export default {
  [CLEAR](state) {
    state.lastCalledAt = undefined;
    state.durationSeconds = undefined;
    state.cookie = undefined;
  },
  [END_VALIDATION_CHECKING](state) {
    state.validationInterval = undefined;
  },
  [HIDE_EXPIRY_MESSAGE](state) {
    delete (state.showExpiryMessage);
  },
  [SET_DURATION_SECONDS](state, numberOfSeconds) {
    state.durationSeconds = numberOfSeconds;
  },
  [SET_LAST_CALLED_AT](state, date) {
    state.lastCalledAt = date;
  },
  [SHOW_EXPIRY_MESSAGE](state) {
    state.showExpiryMessage = true;
  },
  [START_VALIDATION_CHECKING](state, validationInterval) {
    state.validationInterval = validationInterval;
  },
  [SET_CSRF_TOKEN](state, token) {
    state.token = token;
  },
  [SAVE_COOKIE](state, cookie) {
    state.cookie = cookie;
  },
};
