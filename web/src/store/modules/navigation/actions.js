import { redirectTo } from '@/lib/utils';
import { ADVICE_PATH,
  APPOINTMENTS_PATH,
  PRESCRIPTIONS_PATH,
  HEALTH_RECORDS_PATH,
  MESSAGES_PATH,
  ACCOUNT_PATH,
  INDEX_PATH,
  MORE_PATH } from '@/router/paths';
import {
  CLEAR_SELECTED_MENUITEM,
  INIT_NAVIGATION,
  SET_BACK_LINK_OVERRIDE,
  SET_NEWMENUITEM,
  SET_ROUTE_CRUMB,
} from './mutation-types';

export default {
  clearPreviousSelectedMenuItem({ commit }) {
    commit(CLEAR_SELECTED_MENUITEM);
  },
  init({ commit }) {
    commit(INIT_NAVIGATION);
  },
  goTo(_, path) {
    redirectTo({ $router: this.app.$router, $store: this }, path);
  },
  goToAdvicePage() {
    redirectTo({ $router: this.app.$router, $store: this }, ADVICE_PATH);
  },
  goToAppointmentsPage() {
    redirectTo({ $router: this.app.$router, $store: this }, APPOINTMENTS_PATH);
  },
  goToPrescriptionsPage() {
    redirectTo({ $router: this.app.$router, $store: this }, PRESCRIPTIONS_PATH);
  },
  goToYourHealthPage() {
    redirectTo({ $router: this.app.$router, $store: this }, HEALTH_RECORDS_PATH);
  },
  goToMessagesPage() {
    redirectTo({ $router: this.app.$router, $store: this }, MESSAGES_PATH);
  },
  goToSettingsPage() {
    redirectTo({ $router: this.app.$router, $store: this }, ACCOUNT_PATH);
  },
  goToMorePage() {
    redirectTo({ $router: this.app.$router, $store: this }, MORE_PATH);
  },
  goToHomePage() {
    redirectTo({ $router: this.app.$router, $store: this }, INDEX_PATH);
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
  setPreviousMenuItem({ commit, state }) {
    commit(SET_NEWMENUITEM, state.previousMenuItemIndex);
  },
};
