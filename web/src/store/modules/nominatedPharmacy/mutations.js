import assign from 'lodash/fp/assign';
import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
  NOMINATED_PHARMACY_UPDATED,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  SELECT,
  CLEAR_SELECTED_NOMINATED_PHARMACY,
  SET_CHOSEN_TYPE,
  CLEAR_SEARCH_JOURNEY,
  SET_ONLINE_ONLY_KNOWN_OPTION,
  SET_INTERRUPT_BACK_TO,
  CLEAR_INTERRUPT_BACK_TO,
} from './mutation-types';

export default {
  [NOMINATED_PHARMACY_LOADED](state, data) {
    const pharmacy = assign({}, data.pharmacyDetails);
    state.pharmacy = pharmacy;
    state.hasLoaded = true;
    state.nominatedPharmacyEnabled = data.nominatedPharmacyEnabled;
  },
  [NOMINATED_PHARMACY_CLEAR](state) {
    state.pharmacy = {};
    state.hasLoaded = false;
    state.nominatedPharmacyEnabled = null;
    state.justUpdated = false;
  },
  [NOMINATED_PHARMACY_UPDATED](state) {
    state.hasLoaded = false;
    state.justUpdated = true;
  },
  [SET_ONLINE_ONLY_KNOWN_OPTION](state, choice) {
    state.onlineOnlyKnownOption = choice;
  },
  [SET_SEARCH_QUERY](state, searchQuery) {
    state.searchQuery = searchQuery;
  },
  [SET_SEARCH_RESULTS](state, searchResults) {
    state.searchResults = searchResults;
  },
  [CLEAR_SELECTED_NOMINATED_PHARMACY](state) {
    state.selectedNominatedPharmacy = null;
  },
  [SELECT](state, data) {
    state.selectedNominatedPharmacy = data;
  },
  [SET_CHOSEN_TYPE](state, data) {
    state.chosenType = data;
  },
  [CLEAR_SEARCH_JOURNEY](state) {
    state.chosenType = null;
    state.onlineOnlyKnownOption = null;
  },
  [SET_INTERRUPT_BACK_TO](state, data) {
    state.interruptBackTo = data;
  },
  [CLEAR_INTERRUPT_BACK_TO](state) {
    state.interruptBackTo = null;
  },
};
