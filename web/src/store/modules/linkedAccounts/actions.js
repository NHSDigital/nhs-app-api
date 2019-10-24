/* eslint-disable no-unused-vars */
import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_LINKED_ACCOUNT,
  CLEAR_LINKED_ACCOUNTS,
  LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY,
  SET_LINKED_ACCOUNTS_CONFIG,
  SWITCH_TO_LINKED_ACCOUNT,
} from './mutation-types';

export default {
  load({ commit }) {
    this.dispatch('linkedAccounts/clearLinkedAccounts');
    return this.app.$http
      .getV1PatientLinkedAccounts()
      .then((data) => {
        commit(LOADED, data);
      })
      .finally(() => {
        this.dispatch('device/unlockNavBar');
      });
  },
  async initialiseConfig({ commit }) {
    const patientConfigResponse = await this.app.$http.getV1PatientConfiguration();
    if (patientConfigResponse) {
      commit(SET_LINKED_ACCOUNTS_CONFIG, patientConfigResponse.response);
    }
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
  loadAccountAccessSummary({ commit }, id) {
    return this.app.$http
      .getV1PatientLinkedAccountsAccessSummary({ id })
      .then((data) => {
        commit(LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY, data);
      })
      .finally(() => {
        this.dispatch('device/unlockNavBar');
      });
  },
  switchProfile({ commit }, profile) {
    const params = {
      id: profile.id,
    };
    return this.app.$http
      .postV1PatientLinkedAccountsSwitchById(params)
      .then(() => {
        commit(SWITCH_TO_LINKED_ACCOUNT, profile);
      });
  },
};
