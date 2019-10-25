import mapKeys from 'lodash/fp/mapKeys';
import {
  CLEAR,
  LOADED,
  INIT,
  SELECT,
  CLEAR_SELECTED_LINKED_ACCOUNT,
  CLEAR_LINKED_ACCOUNTS,
  LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY,
  SET_LINKED_ACCOUNTS_CONFIG,
  initialState,
} from './mutation-types';

const clearLinkedAccounts = (state) => {
  state.items = [];
  state.hasLoaded = false;
  state.hasErrored = false;
};
const clearSelectedLinkedAccount = (state) => {
  state.selectedLinkedAccount = null;
};
export default {
  [LOADED](state, data) {
    state.items = data.linkedAccounts;
    state.hasLoaded = true;
  },
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
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
  },
};
