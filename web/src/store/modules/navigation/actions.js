import {
  CLEAR_SELECTED_MENUITEM,
  INIT_NAVIGATION,
  SET_BACK_LINK_OVERRIDE,
  SET_NEWMENUITEM,
  SET_ROUTE_CRUMB,
} from './mutation-types';
import { redirectTo } from '@/lib/utils';

export default {
  clearPreviousSelectedMenuItem({ commit }) {
    commit(CLEAR_SELECTED_MENUITEM);
  },
  init({ commit }) {
    commit(INIT_NAVIGATION);
  },
  goTo(_, path) {
    redirectTo({ $router: this.$router, $store: this.app.store }, path);
  },
  setBackLinkOverride({ commit }, backLinkOverride) {
    commit(SET_BACK_LINK_OVERRIDE, backLinkOverride);
  },
  clearBackLinkOverride({ commit }) {
    commit(SET_BACK_LINK_OVERRIDE, undefined);
  },
  setRouteCrumb({ commit }, crumbSetName) {
    commit(SET_ROUTE_CRUMB, crumbSetName);
  },
  setNewMenuItem({ commit }, menuItem) {
    commit(SET_NEWMENUITEM, menuItem);
  },
};
