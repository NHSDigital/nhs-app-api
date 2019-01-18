import {
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  SET_PRIVACY_ACCEPTANCE,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async getReferenceData({ commit }) {
    commit(LOADED_REFERENCE_DATA, await this.app.$http.getV1PatientOrgandonationReferencedata());
  },
  async getRegistration({ commit }) {
    commit(LOADED, await this.app.$http.getV1PatientOrgandonation());
  },
  makeDecision({ commit }, decision) {
    commit(MAKE_DECISION, decision);
  },
  setAllOrgans({ commit }, choice) {
    commit(SET_ALL_ORGANS, choice);
  },
  setAdditionalDetails({ commit }, additionalDetails) {
    commit(SET_ADDITIONAL_DETAILS, additionalDetails);
  },
  toggleAccuracyAcceptance({ commit, state }) {
    commit(SET_ACCURACY_ACCEPTANCE, !state.isAccuracyAccepted);
  },
  togglePrivacyAcceptance({ commit, state }) {
    commit(SET_PRIVACY_ACCEPTANCE, !state.isPrivacyAccepted);
  },
};
