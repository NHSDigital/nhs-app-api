import { PREVENT } from './mutation-types';

export default {
  prevent({ commit }, prevent) {
    commit(PREVENT, prevent);
  },
};
