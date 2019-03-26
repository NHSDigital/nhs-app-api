import { isDefault } from '@/lib/organ-donation/registration-comparison';
import cloneDeep from 'lodash/fp/cloneDeep';
import { ORGAN_DONATION_VIEW_DECISION, ORGAN_DONATION_WITHDRAWN } from '@/lib/routes';
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
  SET_REAFFIRMING,
  SET_REGISTRATION_ID,
  SET_SOME_ORGANS,
  SET_STATE,
  SET_WITHDRAW_REASON_ID,
  SET_WITHDRAWING,
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

const commitData = ({ commit, data, mutation }) => {
  /* Need to check if data is set because server side
     success callback is called even when it fails */
  if (!data) return;
  commit(mutation, data);
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
  async deleteRegistration({ commit, state }) {
    const request = {
      organDonationWithdrawRequest: {
        ...state.originalRegistration,
        withdrawReasonId: state.withdrawReasonId,
      },
    };
    await this.app.$http.deleteV1PatientOrgandonation(request);

    commit(INIT);

    this.$router.push(ORGAN_DONATION_WITHDRAWN.path);
  },
  async getReferenceData({ commit }) {
    return this.app.$http.getV1PatientOrgandonationReferencedata()
      .then(data => commitData({ commit, data, mutation: LOADED_REFERENCE_DATA }));
  },
  async getRegistration({ commit }) {
    return this.app.$http.getV1PatientOrgandonation()
      .then(data => commitData({ commit, data, mutation: LOADED }));
  },
  init({ commit }) {
    commit(INIT);
  },
  makeDecision({ commit }, decision) {
    commit(MAKE_DECISION, decision);
  },
  reaffirmCancel({ commit }) {
    commit(SET_REAFFIRMING, false);
  },
  reaffirmStart({ commit }) {
    commit(SET_REAFFIRMING, true);
  },
  resetAcceptanceChecks({ commit }) {
    commit(SET_PRIVACY_ACCEPTANCE, false);
    commit(SET_ACCURACY_ACCEPTANCE, false);
  },
  setAccuracyAcceptance({ commit }, value) {
    commit(SET_ACCURACY_ACCEPTANCE, value);
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
  setPrivacyAcceptance({ commit }, value) {
    commit(SET_PRIVACY_ACCEPTANCE, value);
  },
  setSomeOrgans({ commit }, { value, choice }) {
    commit(SET_SOME_ORGANS, { value, choice });
  },
  setWithdrawReasonId({ commit }, reasonId) {
    commit(SET_WITHDRAW_REASON_ID, reasonId);
  },
  async submitDecision({ dispatch, state }) {
    return dispatch(`${state.isWithdrawing ? 'delete' : 'submit'}Registration`);
  },
  async submitRegistration({ commit, state }) {
    const request = buildRequest(state);
    const action = state.isAmending || state.isReaffirming ? 'put' : 'post';
    const response = await this.app.$http[`${action}V1PatientOrgandonation`](request);

    commit(SET_STATE, response.state);
    commit(SET_REGISTRATION_ID, response.identifier);
    commit(UPDATE_ORIGINAL_REGISTRATION);

    this.$router.push(ORGAN_DONATION_VIEW_DECISION.path);
  },
  withdrawCancel({ commit }) {
    commit(SET_WITHDRAWING, false);
    commit(SET_WITHDRAW_REASON_ID, '');
  },
  withdrawStart({ commit }) {
    commit(SET_WITHDRAWING, true);
  },
};
