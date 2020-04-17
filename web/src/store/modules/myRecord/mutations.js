/* eslint-disable no-param-reassign */
import { isBlankString, isEmptyArray } from '@/lib/utils';
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
  TOGGLE_PATIENT_DETAIL,
  SET_MEDICAL_RECORD_TYPE,
  SET_RELOAD,
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

function parseCommentsFromDocConsultations(doc) {
  let comments = [];

  doc.consultations.filter(ch => Array.isArray(ch.comments))
    .forEach((ch) => { comments = comments.concat(ch.comments); });

  doc.comments = comments;

  delete doc.consultations;
}

function getConsultationsForDocument(doc, docConsultationsWithComments) {
  const { codeId, term, eventGuid } = doc;

  const docConsultations = docConsultationsWithComments.filter(ch =>
    ch.observationsWithTerm.codeId === codeId &&
    ch.observationsWithTerm.term === term &&
    ch.observationsWithTerm.eventGuid === eventGuid);

  if (!isEmptyArray(docConsultations)) {
    doc.consultations = docConsultations;
  }

  return doc;
}

function parseDocCommentsFromConsultations(state, consultations) {
  let docConsultationsWithComments = [];

  consultations
    .filter(c => !isEmptyArray(c.consultationHeaders))
    .map(c => c.consultationHeaders.filter(ch => ch.header === 'Document'))
    .forEach((c) => { docConsultationsWithComments = docConsultationsWithComments.concat(c); });

  state.record
    .documents
    .data
    .filter(d => isBlankString(d.comments))
    .map(d => getConsultationsForDocument(d, docConsultationsWithComments))
    .filter(d => d.consultations)
    .forEach(d => parseCommentsFromDocConsultations(d));
}

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

    const consultations = record.consultations.data;

    if (Array.isArray(consultations) && !isEmptyArray(consultations)) {
      parseDocCommentsFromConsultations(state, consultations);
    }

    // ensure document comments are always
    // presented as an array in the store
    state.record
      .documents
      .data
      .filter(d => !isBlankString(d.comments))
      .forEach((d) => { d.comments = [d.comments]; });

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
  [SET_RELOAD](state, value) {
    state.reload = value;
  },
  [TOGGLE_PATIENT_DETAIL](state) {
    state.isPatientDetailsCollapsed = !state.isPatientDetailsCollapsed;
  },
  [SET_MEDICAL_RECORD_TYPE](state, { medicalRecordType }) {
    state.medicalRecordType = medicalRecordType;
  },
};
