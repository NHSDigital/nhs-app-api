import mapKeys from 'lodash/fp/mapKeys';
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
  RESET_TERMS,
  TOGGLE_PATIENT_DETAIL,
  SET_MEDICAL_RECORD_TYPE,
  initialState,
} from './mutation-types';


const clearState = (state) => {
  state.hasAcceptedTerms = false;
  state.nojsData = JSON.stringify({ myRecord: { hasAcceptedTerms: false } });
  state.hasLoaded = false;
  state.isPatientDetailsCollapsed = true;
  state.record = {};
  state.patientDetails = {};
  state.detailedTestResult = {
    data: '',
  };
  state.testResults = '';
  state.diagnosis = '';
  state.examinations = '';
  state.procedures = '';
  state.medicalRecordType = undefined;
};

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [CLEAR](state) {
    clearState(state);
  },
  [ACCEPT_TERMS](state) {
    state.hasAcceptedTerms = true;
    state.nojsData = JSON.stringify({ myRecord: { hasAcceptedTerms: true } });
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
  [LOADED_DETAILED_TEST_RESULT](state, { data }) {
    state.detailedTestResult = { data, hasLoaded: true };
  },
  [RESET_TERMS](state) {
    state.hasAcceptedTerms = false;
    state.nojsData = JSON.stringify({ myRecord: { hasAcceptedTerms: false } });
  },
  [TOGGLE_PATIENT_DETAIL](state) {
    state.isPatientDetailsCollapsed = !state.isPatientDetailsCollapsed;
  },
  [SET_MEDICAL_RECORD_TYPE](state, { medicalRecordType }) {
    state.medicalRecordType = medicalRecordType;
  },
};
