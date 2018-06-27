import { UPDATE_HEADER_TEXT, INIT_HEADER } from './mutation-types';

export default {
  updateHeaderText({ commit }, header) {
    commit(UPDATE_HEADER_TEXT, header);
  },
  init({ commit }) {
    commit(INIT_HEADER);
  },
};
