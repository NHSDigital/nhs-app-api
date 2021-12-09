import {
  SET_NOTIFICATION_COOKIE_EXISTS,
  SET_REGISTRATION,
  SET_WAITING,
  TOGGLE_UPDATED,
  NOTIFICATION_COMMUNICATION_ERROR,
} from './mutation-types';

export default {
  [SET_REGISTRATION](state, registered) {
    state.registered = registered;
  },
  [SET_WAITING](state, isWaiting) {
    state.isWaiting = isWaiting;
  },
  [SET_NOTIFICATION_COOKIE_EXISTS](state, exists) {
    state.notificationCookieExists = exists;
  },
  [TOGGLE_UPDATED](state, logged) {
    state.toggleUpdated = logged;
  },
  [NOTIFICATION_COMMUNICATION_ERROR](state, error) {
    state.notificationCommunicationError = error;
  },
};
