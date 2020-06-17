import { TOGGLE_MINI_MENU, CLOSE_MINI_MENU } from './mutation-types';

export default {
  toggleMiniMenu({ commit }) {
    commit(TOGGLE_MINI_MENU);
  },
  closeMiniMenu({ commit }) {
    commit(CLOSE_MINI_MENU);
  },
};
