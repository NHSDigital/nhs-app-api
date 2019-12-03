import {
  INIT,
  SET_IM1_MESSAGING_ENABLED,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  setIm1MessagingEnabled({ commit }, enabled) {
    commit(SET_IM1_MESSAGING_ENABLED, enabled);
  },
};
