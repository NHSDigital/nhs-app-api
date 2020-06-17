import mapPharmacyDetail from '@/lib/pharmacy-detail/mapper';
import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
  NOMINATED_PHARMACY_UPDATED,
  SET_SEARCH_QUERY,
  SET_SEARCH_RESULTS,
  CLEAR_SELECTED_NOMINATED_PHARMACY,
  SELECT,
  SET_CHOSEN_TYPE,
  CLEAR_SEARCH_JOURNEY,
  SET_ONLINE_ONLY_KNOWN_OPTION,
  SET_INTERRUPT_BACK_TO,
  CLEAR_INTERRUPT_BACK_TO,
} from './mutation-types';

const formatWithOpeningTimes = (pharmacyResponse) => {
  const response = Object.assign({}, pharmacyResponse);
  if (pharmacyResponse) {
    // eslint-disable-next-line max-len
    response.openingTimesFormatted = mapPharmacyDetail(pharmacyResponse.openingTimes);
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
        const pharmacyResponse = data;
        pharmacyResponse.pharmacyDetails = formatWithOpeningTimes(data.pharmacyDetails);
        commit(NOMINATED_PHARMACY_LOADED, data);
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
  setOnlineOnlyKnownOption({ commit }, choice) {
    commit(SET_ONLINE_ONLY_KNOWN_OPTION, choice);
  },
  clearSelectedNominatedPharmacy({ commit }) {
    commit(CLEAR_SELECTED_NOMINATED_PHARMACY);
  },
  setChosenType({ commit }, chosenType) {
    commit(SET_CHOSEN_TYPE, chosenType);
  },
  clearSearchJourney({ commit }) {
    commit(CLEAR_SEARCH_JOURNEY);
  },
  setInterruptBackTo({ commit }, backTo) {
    commit(SET_INTERRUPT_BACK_TO, backTo);
  },
  clearInterruptBackTo({ commit }) {
    commit(CLEAR_INTERRUPT_BACK_TO);
  },
};
