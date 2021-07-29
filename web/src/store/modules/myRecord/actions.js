import {
  INIT,
  ACCEPT_TERMS,
  CLEAR,
  LOADED,
  LOADED_TEST_RESULTS,
  LOADED_DIAGNOSIS,
  LOADED_EXAMINATIONS,
  LOADED_PROCEDURES,
  LOADED_DETAILED_TEST_RESULT,
  TOGGLE_PATIENT_DETAIL,
  SET_MEDICAL_RECORD_TYPE,
  SET_RELOAD,
  ADD_ERROR,
} from '@/store/modules/myRecord/mutation-types';
import AnalyticsValues from '@/lib/analytics-values';
import { GP_SESSION_ON_DEMAND_ERROR_STATUS, GP_SESSION_ERROR_STATUS, createLocalError } from '@/lib/utils';

const getMedicalRecordSection = async (commit, $http, mutation, section) => {
  try {
    const { response: record } = await $http.getV1PatientMyRecordSection({ section });
    commit(mutation, { record });
  } catch {
    // Ignoring error as only expected error is unsuccessful response from api
    commit(mutation, {});
  }
};

const loadMedicalRecord = async ({ commit, self, patientDetails }) => {
  const { response: record } =
  await self.app.$http.getV1PatientMyRecord({ ignoreError: true }) || {};
  commit(LOADED, { record, patientDetails });

  let medicalRecordType = AnalyticsValues.NoMedicalRecordAccess;
  if (record && record.hasSummaryRecordAccess) {
    medicalRecordType = record.hasDetailedRecordAccess ?
      AnalyticsValues.SCRAndDCRAccess :
      AnalyticsValues.SCRAccess;
  }
  commit(SET_MEDICAL_RECORD_TYPE, { medicalRecordType });
  self.dispatch('analytics/trackUserProperty', { key: 'medicalRecordType', value: medicalRecordType });
  self.dispatch('analytics/trackUserProperty', { key: 'gpOnlineProduct', value: record.supplier });
};

export default {
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  acceptTerms({ commit }) {
    commit(ACCEPT_TERMS);
  },
  async load({ commit }) {
    try {
      const { response: patientDetails } =
      await this.app.$http.getV1PatientDemographics({ ignoreError: true }) || {};
      if (patientDetails !== undefined) {
        await loadMedicalRecord({ commit, self: this, patientDetails });
      }
      sessionStorage.removeItem('hasRetried');
    } catch (error) {
      if (error.response.status === GP_SESSION_ON_DEMAND_ERROR_STATUS) {
        return;
      }

      if (error.response.status === GP_SESSION_ERROR_STATUS) {
        commit(ADD_ERROR, createLocalError(error));
      } else {
        this.dispatch('errors/addApiError', error);
      }
    } finally {
      this.dispatch('device/unlockNavBar');
    }
  },
  async loadTestResults({ commit }) {
    await getMedicalRecordSection(commit, this.app.$http, LOADED_TEST_RESULTS, 'TestResults');
  },
  async loadDiagnosis({ commit }) {
    await getMedicalRecordSection(commit, this.app.$http, LOADED_DIAGNOSIS, 'Diagnosis');
  },
  async loadExaminations({ commit }) {
    await getMedicalRecordSection(commit, this.app.$http, LOADED_EXAMINATIONS, 'Examinations');
  },
  async loadProcedures({ commit }) {
    await getMedicalRecordSection(commit, this.app.$http, LOADED_PROCEDURES, 'Procedures');
  },
  async loadDetailedTestResult({ commit }, testResultId) {
    const { response: data }
      = await this.app.$http.getV1PatientTestResult({ testResultId }) || {};
    commit(LOADED_DETAILED_TEST_RESULT, { data });
  },
  reload({ commit }, value) {
    commit(SET_RELOAD, value);
  },
  togglePatientDetail({ commit }) {
    commit(TOGGLE_PATIENT_DETAIL);
  },
};
