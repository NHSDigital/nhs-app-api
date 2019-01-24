import {
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  SET_FAITH_DECLARATION,
  SET_PRIVACY_ACCEPTANCE,
  SET_REGISTRATION_ID,
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
  async postRegistration({ commit, state }) {
    const request = {
      organDonationRegistrationRequest: {
        additionalDetails: state.additionalDetails,
        registration: state.registration,
      },
    };
    const { identifier } = await this.app.$http.postV1PatientOrgandonation(request);
    commit(SET_REGISTRATION_ID, identifier);
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
  setFaithDeclaration({ commit }, faithDeclaration) {
    commit(SET_FAITH_DECLARATION, faithDeclaration);
  },
  toggleAccuracyAcceptance({ commit, state }) {
    commit(SET_ACCURACY_ACCEPTANCE, !state.isAccuracyAccepted);
  },
  togglePrivacyAcceptance({ commit, state }) {
    commit(SET_PRIVACY_ACCEPTANCE, !state.isPrivacyAccepted);
  },
};
