import { DISMISS, SYNC, REFRESH_PAGE } from './mutation-types';

export default {
  dismiss({ commit }) {
    commit(DISMISS);
  },
  sync({ commit }) {
    commit(SYNC);
  },
  refreshPage({ commit }) {
    commit(REFRESH_PAGE);
  },
};
