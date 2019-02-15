import { isDefault } from '@/lib/organ-donation/registration-comparison';
import cloneDeep from 'lodash/fp/cloneDeep';
import {
  CLONE_FROM_ORIGINAL,
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_AMENDING,
  SET_ALL_ORGANS,
  SET_FAITH_DECLARATION,
  SET_PRIVACY_ACCEPTANCE,
  SET_REGISTRATION_ID,
  SET_SOME_ORGANS,
  SET_STATE,
  RESET_REGISTRATION,
  UPDATE_ORIGINAL_REGISTRATION,
} from './mutation-types';

const buildRequest = (state) => {
  const registration = cloneDeep(state.registration);
  if (isDefault({ path: 'registration.decisionDetails', state })) {
    registration.decisionDetails = null;
  }

  return {
    organDonationRegistrationRequest: {
      additionalDetails: state.additionalDetails,
      registration,
    },
  };
};

export default {
  amendCancel({ commit }) {
    commit(SET_AMENDING, false);
  },
  amendStart({ commit }) {
    commit(RESET_REGISTRATION);
    commit(SET_AMENDING, true);
    commit(CLONE_FROM_ORIGINAL, [
      'identifier',
      'nameFull',
      'nhsNumber',
      'name',
      'gender',
      'dateOfBirth',
      'addressFull',
      'address',
      'emailAddress',
    ]);
  },
  cloneFromOriginal({ commit }, path) {
    commit(CLONE_FROM_ORIGINAL, path);
  },
  async getReferenceData({ commit }) {
    commit(LOADED_REFERENCE_DATA, await this.app.$http.getV1PatientOrgandonationReferencedata());
  },
  async getRegistration({ commit }) {
    commit(LOADED, await this.app.$http.getV1PatientOrgandonation());
  },
  init({ commit }) {
    commit(INIT);
  },
  makeDecision({ commit }, decision) {
    commit(MAKE_DECISION, decision);
  },
  async postRegistration({ commit, state }) {
    const request = buildRequest(state);
    const response = await this.app.$http.postV1PatientOrgandonation(request);
    commit(SET_STATE, response.state);
    commit(SET_REGISTRATION_ID, response.identifier);
    commit(UPDATE_ORIGINAL_REGISTRATION);
  },
  async putRegistration({ commit, state }) {
    const request = buildRequest(state);
    const response = await this.app.$http.putV1PatientOrgandonation(request);
    commit(SET_STATE, response.state);
    commit(SET_REGISTRATION_ID, response.identifier);
    commit(UPDATE_ORIGINAL_REGISTRATION);
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
  setSomeOrgans({ commit }, { value, choice }) {
    commit(SET_SOME_ORGANS, { value, choice });
  },
  toggleAccuracyAcceptance({ commit, state }) {
    commit(SET_ACCURACY_ACCEPTANCE, !state.isAccuracyAccepted);
  },
  togglePrivacyAcceptance({ commit, state }) {
    commit(SET_PRIVACY_ACCEPTANCE, !state.isPrivacyAccepted);
  },
  resetAcceptanceChecks({ commit }) {
    commit(SET_PRIVACY_ACCEPTANCE, false);
    commit(SET_ACCURACY_ACCEPTANCE, false);
  },
};
