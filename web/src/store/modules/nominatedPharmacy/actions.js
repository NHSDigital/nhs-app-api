import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
} from './mutation-types';

export default {
  load({ commit }) {
    return this.app.$http
      .getV1PatientPharmacyNominated()
      .then((data) => {
        commit(NOMINATED_PHARMACY_LOADED, data);
      });
  },
  clear({ commit }) {
    commit(NOMINATED_PHARMACY_CLEAR);
  },
  setSearchQuery({ commit }, searchQuery) {
    commit(SET_SEARCH_QUERY, searchQuery);
  },
  setSearchResults({ commit }, searchResults) {
    commit(SET_SEARCH_RESULTS, searchResults);
  },
};
