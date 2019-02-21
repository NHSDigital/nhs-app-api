import {
  INIT,
  ACCEPT_TERMS,
  LOADED,
  LOADED_TEST_RESULTS,
  LOADED_DIAGNOSIS,
  LOADED_EXAMINATIONS,
  LOADED_PROCEDURES,
  LOADED_DETAILED_TEST_RESULT,
  RESET_TERMS,
  TOGGLE_PATIENT_DETAIL,
  SET_MEDICAL_RECORD_TYPE,
} from '@/store/modules/myRecord/mutation-types';
import AnalyticsValues from '@/lib/analytics-values';

export default {
  init({ commit }) {
    commit(INIT);
  },
  acceptTerms({ commit }) {
    commit(ACCEPT_TERMS);
  },
  async load({ commit }) {
    const { response: patientDetails } = await this.app.$http.getV1PatientDemographics({}) || {};
    const { response: record } = await this.app.$http.getV1PatientMyRecord({}) || {};
    commit(LOADED, { record, patientDetails });
    let medicalRecordType = AnalyticsValues.NoMedicalRecordAccess;
    if (record && record.hasSummaryRecordAccess) {
      medicalRecordType = record.hasDetailedRecordAccess ?
        AnalyticsValues.SCRAndDCRAccess :
        AnalyticsValues.SCRAccess;
    }
    commit(SET_MEDICAL_RECORD_TYPE, { medicalRecordType });
    if (process.client) {
      this.dispatch('analytics/trackUserProperty', { key: 'medicalRecordType', value: medicalRecordType });
      this.dispatch('analytics/trackUserProperty', { key: 'gpOnlineProduct', value: record.supplier });
    }
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
  async loadExaminations({ commit }) {
    const section = 'Examinations';
    const { response: record } =
      await this.app.$http.getV1PatientMyRecordSection({ section }) || {};
    commit(LOADED_EXAMINATIONS, { record });
  },
  async loadProcedures({ commit }) {
    const section = 'Procedures';
    const { response: record } =
      await this.app.$http.getV1PatientMyRecordSection({ section }) || {};
    commit(LOADED_PROCEDURES, { record });
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
