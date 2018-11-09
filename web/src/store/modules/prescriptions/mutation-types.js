export const PRESCRIPTIONS_LOADED = 'PRESCRIPTIONS_LOADED';
export const PRESCRIPTIONS_CLEAR = 'PRESCRIPTIONS_CLEAR';
export const INIT_PRESCRIPTIONS = 'INIT_PRESCRIPTIONS';
export const initialState = () => ({
  prescriptionCourses: {},
  hasLoaded: false,
  hasErrored: false,
});
