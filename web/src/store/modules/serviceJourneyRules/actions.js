import { INIT, SET_RULES, SET_PATIENT_GUID } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const response = await this.app.$http.getV1PatientJourneyConfiguration();
    if (response) {
      commit(SET_RULES, response);
    }

    const patientConfigResponse = await this.app.$http.getV1PatientConfiguration();
    if (patientConfigResponse) {
      commit(SET_PATIENT_GUID, patientConfigResponse.response.id);
    }
  },
};
