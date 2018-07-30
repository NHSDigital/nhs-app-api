/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { CLEAR_SELECTED_MENUITEM, SET_NEWMENUITEM } from './mutation-types';

/* eslint-disable no-shadow */
function clearPreviousSelectedMenuItem(state) {
  let i;
  const x = [];
  for (i = 0; i < 5; i += 1) {
    x.push(false);
  }
  state.menuItemStatusAt = x;
}

export default {
  [CLEAR_SELECTED_MENUITEM](state) {
    if (process.client && typeof window.nativeApp !== 'undefined') {
      window.nativeApp.clearMenuBarItem();
    } else {
      clearPreviousSelectedMenuItem(state);
    }
  },
  [SET_NEWMENUITEM](state, menuItemIndex) {
    clearPreviousSelectedMenuItem(state);
    state.menuItemStatusAt[menuItemIndex] = true;
  },
};
