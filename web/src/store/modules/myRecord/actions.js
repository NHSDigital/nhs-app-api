import {
  ACCEPT_TERMS,
  LOADED,
  LOADED_TEST_RESULTS,
  LOADED_DIAGNOSIS,
  LOADED_DETAILED_TEST_RESULT,
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
  async loadTestResults({ commit }) {
    const section = 'TestResults';
    const { response: record } =
      await this.app.$http.getV1PatientMyRecordSection({ section }) || {};
    commit(LOADED_TEST_RESULTS, { record });
  },
  async loadDiagnosis({ commit }) {
    const section = 'Diagnosis';
    const { response: record } =
      await this.app.$http.getV1PatientMyRecordSection({ section }) || {};
    commit(LOADED_DIAGNOSIS, { record });
  },
  async loadDetailedTestResult({ commit }, testResultId) {
    const { response } = await this.app.$http.getV1PatientTestResult({ testResultId }) || {};
    const { hasErrored, testResult: data } = response;
    commit(LOADED_DETAILED_TEST_RESULT, { hasErrored, data });
  },
  resetTerms({ commit }) {
    commit(RESET_TERMS);
  },
  togglePatientDetail({ commit }) {
    commit(TOGGLE_PATIENT_DETAIL);
  },
};
