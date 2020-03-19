export const INIT = 'INIT';
export const ACCEPT_TERMS = 'ACCEPT_TERMS';
export const CLEAR = 'CLEAR';
export const LOADED = 'LOADED';
export const LOADED_TEST_RESULTS = 'LOADED_TEST_RESULTS';
export const LOADED_DIAGNOSIS = 'LOADED_DIAGNOSIS';
export const LOADED_EXAMINATIONS = 'LOADED_EXAMINATIONS';
export const LOADED_PROCEDURES = 'LOADED_PROCEDURES';
export const LOADED_DETAILED_TEST_RESULT = 'LOADED_DETAILED_TEST_RESULT';
export const LOADED_DOCUMENT = 'LOADED_DOCUMENT';
export const SET_MEDICAL_RECORD_TYPE = 'SET_MEDICAL_RECORD_TYPE';
export const SET_RELOAD = 'SET_RELOAD';
export const SET_SELECTED_DOCUMENT_INFO = 'SET_SELECTED_DOCUMENT_INFO';
export const TOGGLE_PATIENT_DETAIL = 'TOGGLE_PATIENT_DETAIL';
export const SET_VALID_FILE = 'SET_VALID_FILE';
export const initialState = () => ({
  hasAcceptedTerms: false,
  nojsData: JSON.stringify({ myRecord: { hasAcceptedTerms: false } }),
  hasLoaded: false,
  reload: true,
  isPatientDetailsCollapsed: true,
  record: {},
  patientDetails: {},
  detailedTestResult: {
    data: '',
  },
  testResults: '',
  diagnosis: '',
  examinations: '',
  procedures: '',
  medicalRecordType: undefined,
  document: {},
  documentConsultationsWithComments: [],
});
