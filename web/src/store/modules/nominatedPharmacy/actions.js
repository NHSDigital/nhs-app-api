import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
  NOMINATED_PHARMACY_UPDATED,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  CLEAR_SELECTED_NOMINATED_PHARMACY,
  SELECT,
} from './mutation-types';

export default {
  load({ commit }) {
    return this.app.$http
      .getV1PatientNominatedPharmacy()
      .then((data) => {
        commit(NOMINATED_PHARMACY_LOADED, data);
      });
  },
  update({ commit }, data) {
    const request = {
      odsCode: data,
    };
    const param = {
      updateNominatedPharmacyRequest: request,
    };
    return this.app.$http
      .postV1PatientNominatedPharmacy(param)
      .then(() => {
        commit(NOMINATED_PHARMACY_UPDATED);
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
  select({ commit }, nominatedPharmacy) {
    commit(SELECT, nominatedPharmacy);
  },
  clearSelectedNominatedPharmacy({ commit }) {
    commit(CLEAR_SELECTED_NOMINATED_PHARMACY);
  },
};
