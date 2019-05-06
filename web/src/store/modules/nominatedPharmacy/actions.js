import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
  NOMINATED_PHARMACY_UPDATED,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  CLEAR_SELECTED_NOMINATED_PHARMACY,
  SELECT,
  SET_PREVIOUS_PAGE_TO_SEARCH,
} from './mutation-types';
import mapPharmacyDetail from '@/lib/pharmacy-detail/mapper';

const formatWithOpeningTimes = (pharmacyResponse) => {
  const response = Object.assign({}, pharmacyResponse);
  if (pharmacyResponse.pharmacyDetails) {
    // eslint-disable-next-line max-len
    response.pharmacyDetails.openingTimesFormatted = mapPharmacyDetail(pharmacyResponse.pharmacyDetails.openingTimes);
  }
  return response;
};

export default {
  load({ commit }) {
    return this.app.$http
      .getV1PatientNominatedPharmacy({
        ignoreError: true,
      })
      .then((data) => {
        commit(NOMINATED_PHARMACY_LOADED, formatWithOpeningTimes(data));
      })
      .catch(() => {});
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
    commit(SELECT, formatWithOpeningTimes(nominatedPharmacy));
  },
  clearSelectedNominatedPharmacy({ commit }) {
    commit(CLEAR_SELECTED_NOMINATED_PHARMACY);
  },
  setPreviousPageToSearch({ commit }, previousPagePath) {
    commit(SET_PREVIOUS_PAGE_TO_SEARCH, previousPagePath);
  },
};
