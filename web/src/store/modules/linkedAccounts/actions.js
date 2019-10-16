/* eslint-disable no-unused-vars */
import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_LINKED_ACCOUNT,
  CLEAR_LINKED_ACCOUNTS,
} from './mutation-types';

export default {
  load({ commit }) {
    this.dispatch('linkedAccounts/init');
    return this.app.$http
      .getV1PatientLinkedAccounts()
      .then((data) => {
        commit(LOADED, data);
      })
      .finally(() => {
        this.dispatch('device/unlockNavBar');
      });
  },
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  select({ commit }, linkedAccount) {
    commit(SELECT, linkedAccount);
  },
  clearSelectedLinkedAccount({ commit }) {
    commit(CLEAR_SELECTED_LINKED_ACCOUNT);
  },
  clearLinkedAccounts({ commit }) {
    commit(CLEAR_LINKED_ACCOUNTS);
  },
};
