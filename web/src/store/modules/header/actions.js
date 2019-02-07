import { UPDATE_HEADER_TEXT, INIT_HEADER, TOGGLE_MINI_MENU, CLOSE_MINI_MENU } from './mutation-types';

export default {
  updateHeaderText({ commit }, header) {
    commit(UPDATE_HEADER_TEXT, header);
  },
  init({ commit }) {
    commit(INIT_HEADER);
  },
  toggleMiniMenu({ commit }) {
    commit(TOGGLE_MINI_MENU);
  },
  closeMiniMenu({ commit }) {
    commit(CLOSE_MINI_MENU);
  },
};
