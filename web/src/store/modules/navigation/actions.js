import consola from 'consola';
import { APPOINTMENTS, HEALTH_RECORDS, INDEX, MESSAGES, PRESCRIPTIONS, SYMPTOMS } from '@/lib/routes';
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
  goToPage({ dispatch }, page) {
    let route;

    switch (page) {
      case window.nhsapp.navigation.AppPage.HOME_PAGE:
        route = INDEX;
        break;
      case window.nhsapp.navigation.AppPage.APPOINTMENTS:
        route = APPOINTMENTS;
        break;
      case window.nhsapp.navigation.AppPage.PRESCRIPTIONS:
        route = PRESCRIPTIONS;
        break;
      case window.nhsapp.navigation.AppPage.HEALTH_RECORDS:
        route = HEALTH_RECORDS;
        break;
      case window.nhsapp.navigation.AppPage.SYMPTOMS:
        route = SYMPTOMS;
        break;
      case window.nhsapp.navigation.AppPage.MESSAGES:
        route = MESSAGES;
        break;
      default:
        consola.error(new Error(`Invalid navigation page enum value: ${page}`));
        return;
    }

    dispatch('goTo', route.path);
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
