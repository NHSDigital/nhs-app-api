/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import NativeCallbacks from '@/services/native-app';
import {
  CLEAR_SELECTED_MENUITEM,
  SET_NEWMENUITEM,
  INIT_NAVIGATION,
  SET_BACK_LINK_OVERRIDE,
  SET_ROUTE_CRUMB,
} from './mutation-types';

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
    if (process.client && window.nativeApp) {
      NativeCallbacks.clearMenuBarItem();
    } else {
      clearPreviousSelectedMenuItem(state);
    }
  },
  [SET_NEWMENUITEM](state, menuItemIndex) {
    if (process.client && window.nativeApp !== 'undefined') {
      NativeCallbacks.setMenuBarItem(menuItemIndex);
    }
    clearPreviousSelectedMenuItem(state);
    state.menuItemStatusAt[menuItemIndex] = true;
  },
  [INIT_NAVIGATION](state) {
    state.menuItemStatusAt = [];
  },
  [SET_BACK_LINK_OVERRIDE](state, backLinkOverride) {
    state.backLinkOverride = backLinkOverride;
  },
  [SET_ROUTE_CRUMB](state, crumbSetName) {
    state.crumbSetName = crumbSetName;
  },
};
