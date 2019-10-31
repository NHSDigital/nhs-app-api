import { DISMISS, SYNC } from './mutation-types';

export default {
  dismiss({ commit }) {
    commit(DISMISS);
  },
  sync({ commit }) {
    commit(SYNC);
  },
};
