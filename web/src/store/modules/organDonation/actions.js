import { LOADED, LOADED_REFERENCE_DATA, MAKE_DECISION, SET_ALL_ORGANS } from './mutation-types';

export default {
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
};
