import {
  CLEAR_SELECTED_MENUITEM,
  SET_NEWMENUITEM,
} from './mutation-types';

function clearPreviousSelectedMenuItem(state) {
  if (state.previousSelectedMenuItem && state.previousSelectedMenuItem.classList) {
    state.previousSelectedMenuItem.classList.remove('active');
  }
}

export default {
  [CLEAR_SELECTED_MENUITEM](state) {
    clearPreviousSelectedMenuItem(state);
  },
  [SET_NEWMENUITEM](state, menuItem) {
    clearPreviousSelectedMenuItem(state);
    menuItem.classList.add('active');
    state.previousSelectedMenuItem = menuItem;
  },
};
