import {
  CLEAR_SELECTED_MENUITEM,
  SET_NEWMENUITEM,
} from './mutation-types';

export const clearPreviousSelectedMenuItem = ({ commit }) => {
  commit(CLEAR_SELECTED_MENUITEM);
};

export const setNewMenuItem = ({ commit }, menuItem) => {
  commit(SET_NEWMENUITEM, menuItem);
};

export default {
  clearPreviousSelectedMenuItem,
  setNewMenuItem,
};
