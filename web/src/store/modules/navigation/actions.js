import { CLEAR_SELECTED_MENUITEM, SET_NEWMENUITEM, INIT_NAVIGATION } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT_NAVIGATION);
  },
  clearPreviousSelectedMenuItem({ commit }) {
    commit(CLEAR_SELECTED_MENUITEM);
  },
  setNewMenuItem({ commit }, menuItem) {
    commit(SET_NEWMENUITEM, menuItem);
  },
};
