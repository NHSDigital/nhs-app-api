export const ACCEPT_TERMS = 'ACCEPT_TERMS';
export const LOADED = 'LOADED';
export const LOADED_TEST_RESULTS = 'LOADED_TEST_RESULTS';
export const LOADED_DIAGNOSIS = 'LOADED_DIAGNOSIS';
export const LOADED_DETAILED_TEST_RESULT = 'LOADED_DETAILED_TEST_RESULT';
export const RESET_TERMS = 'RESET_TERMS';
export const TOGGLE_PATIENT_DETAIL = 'TOGGLE_PATIENT_DETAIL';
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
});
