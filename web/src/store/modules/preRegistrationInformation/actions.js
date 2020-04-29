import { CONTINUE, SYNC } from './mutation-types';

export default {
  continue({ commit }) {
    commit(CONTINUE);
  },
  sync({ commit }) {
    commit(SYNC);
  },
};
