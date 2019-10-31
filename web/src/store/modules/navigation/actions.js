import { redirectTo } from '@/lib/utils';
import {
  CLEAR_SELECTED_MENUITEM,
  INIT_NAVIGATION,
  SET_BACK_LINK_OVERRIDE,
  SET_NEWMENUITEM,
} from './mutation-types';

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
  setNewMenuItem({ commit }, menuItem) {
    commit(SET_NEWMENUITEM, menuItem);
  },
};
