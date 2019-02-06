export const INIT = 'INIT';
export const ACCEPT_TERMS = 'ACCEPT_TERMS';
export const LOADED = 'LOADED';
export const LOADED_TEST_RESULTS = 'LOADED_TEST_RESULTS';
export const LOADED_DIAGNOSIS = 'LOADED_DIAGNOSIS';
export const LOADED_EXAMINATIONS = 'LOADED_EXAMINATIONS';
export const LOADED_PROCEDURES = 'LOADED_PROCEDURES';
export const LOADED_DETAILED_TEST_RESULT = 'LOADED_DETAILED_TEST_RESULT';
export const RESET_TERMS = 'RESET_TERMS';
export const TOGGLE_PATIENT_DETAIL = 'TOGGLE_PATIENT_DETAIL';
export const SET_MEDICAL_RECORD_TYPE = 'SET_MEDICAL_RECORD_TYPE';
export const initialState = () => ({
  hasAcceptedTerms: false,
  hasLoaded: false,
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
});
