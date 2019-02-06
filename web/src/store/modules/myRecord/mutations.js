import mapKeys from 'lodash/fp/mapKeys';
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
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [ACCEPT_TERMS](state) {
    state.hasAcceptedTerms = true;
  },
  [LOADED](state, { record, patientDetails }) {
    state.record = record;
    state.patientDetails = patientDetails;
    state.hasLoaded = true;
    state.isPatientDetailsCollapsed = false;
  },
  [LOADED_TEST_RESULTS](state, { record }) {
    state.testResults = record;
  },
  [LOADED_DIAGNOSIS](state, { record }) {
    state.diagnosis = record;
  },
  [LOADED_EXAMINATIONS](state, { record }) {
    state.examinations = record;
  },
  [LOADED_PROCEDURES](state, { record }) {
    state.procedures = record;
  },
  [LOADED_DETAILED_TEST_RESULT](state, { hasErrored, data }) {
    state.detailedTestResult = { data, hasErrored, hasLoaded: true };
  },
  [RESET_TERMS](state) {
    state.hasAcceptedTerms = false;
  },
  [TOGGLE_PATIENT_DETAIL](state) {
    state.isPatientDetailsCollapsed = !state.isPatientDetailsCollapsed;
  },
  [SET_MEDICAL_RECORD_TYPE](state, { medicalRecordType }) {
    state.medicalRecordType = medicalRecordType;
  },
};
