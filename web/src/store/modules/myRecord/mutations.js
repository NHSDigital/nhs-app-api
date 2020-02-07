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
  LOADED_DOCUMENT,
  TOGGLE_PATIENT_DETAIL,
  SET_MEDICAL_RECORD_TYPE,
  SET_RELOAD,
  SET_SELECTED_DOCUMENT_INFO,
  initialState,
} from './mutation-types';

const clearState = (state) => {
  state.hasAcceptedTerms = false;
  state.nojsData = JSON.stringify({ myRecord: { hasAcceptedTerms: false } });
  state.hasLoaded = false;
  state.reload = true;
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
  state.documentConsultationsWithComments = [];
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
    state.isPatientDetailsCollapsed = false;
    state.record.documents.data = (record.documents.data || []);
    state.record.documents.recordCount = state.record.documents.data.length;

    if (state.record.medications && state.record.medications.hasErrored) {
      state.record.medications.data.acuteMedications.hasErrored = true;
      state.record.medications.data.currentRepeatMedications.hasErrored = true;
      state.record.medications.data.discontinuedRepeatMedications.hasErrored = true;
    }

    state.documentConsultationsWithComments = (record.consultations.data || [])
      .filter(d => d.consultationHeaders.filter(p => p.header === 'Document' || p.header === 'Comment').length > 0)
      .filter(x => x.consultationHeaders.length > 1);

    state.hasLoaded = true;
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
  [LOADED_DOCUMENT](state, documentData) {
    state.document.data = documentData;
  },
  [SET_RELOAD](state, value) {
    state.reload = value;
  },
  [SET_SELECTED_DOCUMENT_INFO](state, documentInfo) {
    if (!state.document) {
      state.document = documentInfo;
    } else {
      state.document.name = documentInfo.name;
      state.document.type = documentInfo.type;
      state.document.date = documentInfo.date;
      state.document.term = documentInfo.term;
      state.document.eventGuid = documentInfo.eventGuid;
      state.document.codeId = documentInfo.codeId;
      state.document.size = documentInfo.size;
      state.document.isValidFile = documentInfo.isValidFile;
    }
  },
  [TOGGLE_PATIENT_DETAIL](state) {
    state.isPatientDetailsCollapsed = !state.isPatientDetailsCollapsed;
  },
  [SET_MEDICAL_RECORD_TYPE](state, { medicalRecordType }) {
    state.medicalRecordType = medicalRecordType;
  },
};
