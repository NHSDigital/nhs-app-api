import {
  assign,
} from 'lodash/fp';
import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
  NOMINATED_PHARMACY_UPDATED,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  SELECT,
  CLEAR_SELECTED_NOMINATED_PHARMACY,
  SET_PREVIOUS_PAGE_TO_SEARCH,
} from './mutation-types';

export default {
  [NOMINATED_PHARMACY_LOADED](state, data) {
    const pharmacy = assign({}, data);

    state.pharmacy = pharmacy;
    state.hasLoaded = true;
  },
  [NOMINATED_PHARMACY_CLEAR](state) {
    state.pharmacy = {};
    state.hasLoaded = false;
  },
  [NOMINATED_PHARMACY_UPDATED](state) {
    state.hasLoaded = false;
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
  [SET_PREVIOUS_PAGE_TO_SEARCH](state, data) {
    state.previousPageToSearch = data;
  },
};
