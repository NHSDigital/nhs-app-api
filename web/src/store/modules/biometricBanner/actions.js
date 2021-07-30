import { DISMISS, SYNC } from './mutation-types';

export default {
  dismiss({ commit }) {
    commit(DISMISS);
    commit(SYNC);
  },
  sync({ commit }) {
    commit(SYNC);
  },
};
