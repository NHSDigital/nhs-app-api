import { SET_REGISTRATION, SET_SETTINGS_ENABLED, SET_WAITING } from './mutation-types';

export default {
  [SET_REGISTRATION](state, registered) {
    state.registered = registered;
  },
  [SET_SETTINGS_ENABLED](state, isEnabled) {
    state.isSettingsEnabled = isEnabled;
  },
  [SET_WAITING](state, isWaiting) {
    state.isWaiting = isWaiting;
  },
};
