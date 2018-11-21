import {
  ACCEPT_TERMS,
  LOADED,
  LOADED_TEST_RESULTS,
  RESET_TERMS,
  TOGGLE_PATIENT_DETAIL,
} from '@/store/modules/myRecord/mutation-types';

export default {
  acceptTerms({ commit }) {
    commit(ACCEPT_TERMS);
  },
  async load({ commit }) {
    const { response: patientDetails } = await this.app.$http.getV1PatientDemographics({}) || {};
    const { response: record } = await this.app.$http.getV1PatientMyRecord({}) || {};
    commit(LOADED, { record, patientDetails });
  },
  async loadTestResults({ commit }, testResultId) {
    const { response } = await this.app.$http.getV1PatientTestResult({ testResultId }) || {};
    const { hasErrored, testResult: data } = response;
    commit(LOADED_TEST_RESULTS, { hasErrored, data });
  },
  resetTerms({ commit }) {
    commit(RESET_TERMS);
  },
  togglePatientDetail({ commit }) {
    commit(TOGGLE_PATIENT_DETAIL);
  },
};
