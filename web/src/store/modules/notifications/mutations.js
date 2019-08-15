import { SET_REGISTRATION, SET_WAITING } from './mutation-types';

export default {
  [SET_REGISTRATION](state, registered) {
    state.registered = registered;
  },
  [SET_WAITING](state, isWaiting) {
    state.isWaiting = isWaiting;
  },
};
