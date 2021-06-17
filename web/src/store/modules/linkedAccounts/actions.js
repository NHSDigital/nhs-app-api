/* eslint-disable no-unused-vars */
import { INDEX_NAME } from '@/router/names';
import {
  GP_SESSION_ERROR_STATUS,
  GP_SESSION_ON_DEMAND_ERROR_STATUS,
  createLocalError,
  redirectByName,
} from '@/lib/utils';
import {
  CLEAR,
  LOADED,
  INIT,
  LOSS_PROXY,
  LOSS_PROXY_RESET,
  SELECT,
  CLEAR_SELECTED_LINKED_ACCOUNT,
  CLEAR_LINKED_ACCOUNTS,
  LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY,
  SET_LINKED_ACCOUNTS_CONFIG,
  SWITCH_TO_LINKED_ACCOUNT,
  SWITCH_TO_MAIN_USER_ACCOUNT,
  ADD_ERROR,
} from './mutation-types';

export default {
  async load({ commit }) {
    this.dispatch('linkedAccounts/clearLinkedAccounts');
    try {
      const response = await this.app.$http.getV1PatientLinkedAccounts({
        ignoreError: true,
        returnResponse: true,
      });
      commit(LOADED, response.data);
    } catch (error) {
      // Do nothing
    } finally {
      this.dispatch('device/unlockNavBar');
    }
  },
  async initialiseConfig({ commit }) {
    commit(CLEAR);
    try {
      const patientConfigResponse = await this.app.$http.getV1PatientConfiguration({
        ignoreError: true,
      });
      commit(SET_LINKED_ACCOUNTS_CONFIG, patientConfigResponse);

      sessionStorage.removeItem('hasRetried');
    } catch (error) {
      if (error.response && error.response.status !== GP_SESSION_ON_DEMAND_ERROR_STATUS) {
        if (error.response.status !== GP_SESSION_ERROR_STATUS) {
          this.dispatch('errors/addApiError', error);
        } else {
          commit(ADD_ERROR, createLocalError(error));
        }
      }
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
        this.dispatch('myRecord/clear');
        this.dispatch('serviceJourneyRules/init');
      });
  },
  switchToMainUserProfile({ commit, getters }) {
    const { mainPatientId } = getters;
    const params = {
      id: mainPatientId,
    };
    return this.app.$http
      .postV1PatientLinkedAccountsSwitchById(params)
      .then(() => {
        commit(SWITCH_TO_MAIN_USER_ACCOUNT);
      });
  },
  redirectAfterInvalidPatientIdDetected({ commit }) {
    commit(LOSS_PROXY);
    redirectByName({ $router: this.app.$router, $store: this }, INDEX_NAME);
  },
  proxyRecoveryComplete({ commit }) {
    commit(LOSS_PROXY_RESET);
  },
};
