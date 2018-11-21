import {
  CLEAR,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
} from './mutation-types';

export default {
  [CLEAR](state) {
    state.lastCalledAt = undefined;
    state.durationSeconds = undefined;
    state.gpOdsCode = undefined;
  },
  [END_VALIDATION_CHECKING](state) {
    state.validationInterval = undefined;
  },
  [HIDE_EXPIRY_MESSAGE](state) {
    delete (state.showExpiryMessage);
  },
  [SET_INFO](state, {
    name,
    durationSeconds,
    gpOdsCode,
    sessionTimeout,
    token,
    lastCalledAt = new Date(),
    nhsNumber,
    dateOfBirth,
    accessToken,
  } = {}) {
    state.user = name;
    state.durationSeconds = durationSeconds;
    state.gpOdsCode = gpOdsCode;
    state.sessionTimeout = sessionTimeout;
    state.csrfToken = token;
    state.lastCalledAt = lastCalledAt;
    state.nhsNumber = nhsNumber;
    state.dateOfBirth = dateOfBirth;
    state.accessToken = accessToken;
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
};
