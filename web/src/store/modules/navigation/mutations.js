import NativeApp from '@/services/native-app';
import {
  CLEAR_SELECTED_MENUITEM,
  SET_NEWMENUITEM,
  INIT_NAVIGATION,
  SET_BACK_LINK_OVERRIDE,
  SET_ROUTE_CRUMB,
} from './mutation-types';

function clearPreviousSelectedMenuItem(state) {
  state.menuItemStatusAt = Array(5).fill(false);
}

export default {
  [CLEAR_SELECTED_MENUITEM](state) {
    if (window.nativeApp) {
      NativeApp.clearMenuBarItem();
    } else {
      clearPreviousSelectedMenuItem(state);
    }
  },
  [SET_NEWMENUITEM](state, menuItemIndex) {
    if (window.nativeApp !== 'undefined') {
      NativeApp.setMenuBarItem(menuItemIndex);
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
