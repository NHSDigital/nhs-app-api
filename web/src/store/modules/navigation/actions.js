import {
  CLEAR_SELECTED_MENUITEM,
  SET_NEWMENUITEM,
  INIT_NAVIGATION,
  SET_BACK_LINK_OVERRIDE,
} from './mutation-types';

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
  setBackLinkOverride({ commit }, backLinkOverride) {
    commit(SET_BACK_LINK_OVERRIDE, backLinkOverride);
  },
};
