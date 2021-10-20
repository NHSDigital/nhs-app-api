import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  CLEAR,
  LOADED,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  HIDE_SESSION_EXPIRING,
  SET_INFO,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SHOW_SESSION_EXPIRING,
  SET_USER_SESSION_REFERENCE,
  SET_RETRY_GP_SESSION,
  HAS_GP_SESSION,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      if (key !== 'hasRetried') {
        state[key] = blank[key];
      }
    })(state);
  },
  [CLEAR](state) {
    state.lastCalledAt = undefined;
    state.durationSeconds = undefined;
    state.gpOdsCode = undefined;
  },
  [LOADED](state) {
    state.hasLoaded = true;
  },
  [END_VALIDATION_CHECKING](state) {
    state.validationInterval = undefined;
  },
  [HIDE_EXPIRY_MESSAGE](state) {
    delete (state.showExpiryMessage);
  },
  [SET_INFO](state, {
    user,
    durationSeconds,
    gpOdsCode,
    sessionTimeout,
    csrfToken,
    lastCalledAt,
    nhsNumber,
    dateOfBirth,
    accessToken,
    proofLevel,
  } = {}) {
    state.user = user;
    state.durationSeconds = durationSeconds;
    state.gpOdsCode = gpOdsCode;
    state.sessionTimeout = sessionTimeout;
    state.csrfToken = csrfToken;
    state.lastCalledAt = lastCalledAt;
    state.nhsNumber = nhsNumber;
    state.dateOfBirth = dateOfBirth;
    state.accessToken = accessToken;
    state.proofLevel = proofLevel;
  },
  [SET_USER_SESSION_REFERENCE](state, userSessionCreateReferenceCode) {
    state.userSessionCreateReferenceCode = userSessionCreateReferenceCode;
  },
  [SHOW_EXPIRY_MESSAGE](state) {
    state.showExpiryMessage = true;
  },
  [START_VALIDATION_CHECKING](state, validationInterval) {
    state.validationInterval = validationInterval;
  },
  [SHOW_SESSION_EXPIRING](state) {
    state.showSessionExpiring = true;
  },
  [HIDE_SESSION_EXPIRING](state) {
    delete (state.showSessionExpiring);
  },
  [SET_RETRY_GP_SESSION](state, hasRetried) {
    state.hasRetried = hasRetried;
  },
  [HAS_GP_SESSION](state, hasGpSession) {
    state.hasGpSession = hasGpSession;
  },
};
