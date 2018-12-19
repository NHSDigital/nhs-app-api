import { LOADED, MAKE_DECISION } from './mutation-types';

export default {
  async getRegistration({ commit }) {
    const result = await this.app.$http.getV1PatientOrgandonation();
    commit(LOADED, result);
  },
  makeDecision({ commit }, decision) {
    commit(MAKE_DECISION, decision);
  },
};
