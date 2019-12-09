import mapKeys from 'lodash/fp/mapKeys';
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
  initialState,
} from './mutation-types';

const clearLinkedAccounts = (state) => {
  state.items = [];
  state.hasErrored = false;
};
const clearSelectedLinkedAccount = (state) => {
  state.selectedLinkedAccount = null;
};
const init = (state) => {
  const blank = initialState();
  mapKeys((key) => {
    state[key] = blank[key];
  })(state);
};
export default {
  [LOADED](state, data) {
    state.items = data.linkedAccounts;
  },
  [INIT](state) {
    init(state);
  },
  [CLEAR](state) {
    clearLinkedAccounts(state);
    clearSelectedLinkedAccount(state);
  },
  [SELECT](state, selected) {
    state.selectedLinkedAccount = selected;
  },
  [CLEAR_SELECTED_LINKED_ACCOUNT](state) {
    clearSelectedLinkedAccount(state);
  },
  [CLEAR_LINKED_ACCOUNTS](state) {
    clearLinkedAccounts(state);
  },
  [LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY](state, data) {
    Object.assign(state.selectedLinkedAccount, data);
  },
  [SET_LINKED_ACCOUNTS_CONFIG](state, config) {
    state.config.hasLoaded = true;
    state.config.patientId = config.id;
    state.config.hasLinkedAccounts = config.hasLinkedAccounts;
    state.items = config.linkedAccounts;
  },
  [SWITCH_TO_LINKED_ACCOUNT](state, profile) {
    state.actingAsUser = profile;
  },
  [SWITCH_TO_MAIN_USER_ACCOUNT](state) {
    state.actingAsUser = null;
  },
  [LOSS_PROXY](state) {
    init(state);
    state.recoverFromProxyLoss = true;
  },
  [LOSS_PROXY_RESET](state) {
    state.recoverFromProxyLoss = false;
  },
};
