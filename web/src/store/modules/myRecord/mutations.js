import {
  ACCEPT_TERMS,
  LOADED,
  LOADED_TEST_RESULTS,
  RESET_TERMS,
  TOGGLE_PATIENT_DETAIL,
} from './mutation-types';

export default {
  [ACCEPT_TERMS](state) {
    state.hasAcceptedTerms = true;
  },
  [LOADED](state, { record, patientDetails }) {
    state.record = record;
    state.patientDetails = patientDetails;
    state.hasLoaded = true;
    state.isPatientDetailsCollapsed = false;
  },
  [LOADED_TEST_RESULTS](state, { hasErrored, data }) {
    state.testResults = { data, hasErrored, hasLoaded: true };
  },
  [RESET_TERMS](state) {
    state.hasAcceptedTerms = false;
  },
  [TOGGLE_PATIENT_DETAIL](state) {
    state.isPatientDetailsCollapsed = !state.isPatientDetailsCollapsed;
  },
};
