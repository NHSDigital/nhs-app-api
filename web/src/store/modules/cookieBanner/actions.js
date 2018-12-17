import { ACKNOWLEDGE, SYNC } from './mutation-types';

export default {
  acknowledge({ commit }) {
    commit(ACKNOWLEDGE);
  },
  sync({ commit }) {
    commit(SYNC);
  },
};
