import {
  CLEAR,
  HIDE_EXPIRY_MESSAGE,
  SET_DURATION_SECONDS,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
} from './mutation-types';

export default {
  [CLEAR](state) {
    state.lastCalledAt = undefined;
    state.durationSeconds = undefined;
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
};
