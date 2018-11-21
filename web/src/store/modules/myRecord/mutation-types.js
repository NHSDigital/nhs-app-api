export const ACCEPT_TERMS = 'ACCEPT_TERMS';
export const LOADED = 'LOADED';
export const LOADED_TEST_RESULTS = 'LOADED_TEST_RESULTS';
export const RESET_TERMS = 'RESET_TERMS';
export const TOGGLE_PATIENT_DETAIL = 'TOGGLE_PATIENT_DETAIL';
export const initialState = () => ({
  hasAcceptedTerms: false,
  hasLoaded: false,
  isPatientDetailsCollapsed: true,
  record: {},
  patientDetails: {},
  testResults: {
    data: '',
  },
});
