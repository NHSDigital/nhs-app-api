import { INIT, SET_RULES } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const response = await this.app.$http.getV1PatientJourneyConfiguration();
    if (response) {
      commit(SET_RULES, response);
    }
  },
  async loadLinkedAccount({ commit }) {
    const response = await this.app.$http.getV1PatientLinkedAccountJourneyConfiguration();
    if (response) {
      commit(SET_RULES, response);
    }
  },
};
