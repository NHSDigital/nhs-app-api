export const PRESCRIPTIONS_LOADED = 'PRESCRIPTIONS_LOADED';
export const PRESCRIPTIONS_CLEAR = 'PRESCRIPTIONS_CLEAR';
export const ADD_ERROR = 'ADD_ERROR';
export const initialState = () => ({
  prescriptionCourses: {},
  hasLoaded: false,
  hasErrored: false,
  error: undefined,
});
