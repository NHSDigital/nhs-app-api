export const PRESCRIPTIONS_LOADED = 'PRESCRIPTIONS_LOADED';
export const PRESCRIPTIONS_CLEAR = 'PRESCRIPTIONS_CLEAR';
export const initialState = () => ({
  prescriptionCourses: {},
  hasLoaded: false,
  hasErrored: false,
});
