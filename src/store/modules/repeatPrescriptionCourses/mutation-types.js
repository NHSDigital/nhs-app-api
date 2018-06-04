export const REPEAT_PRESCRIPTION_COURSES_LOADED =
  'REPEAT_PRESCRIPTION_COURSES_LOADED';
export const INIT_REPEAT_PRESCRIPTIONS = 'INIT_REPEAT_PRESCRIPTIONS';
export const SELECT_REPEAT_PRESCRIPTION = 'SELECT_REPEAT_PRESCRIPTION';
export const REPEAT_PRESCRIPTION_VALIDATED = 'REPEAT_PRESCRIPTION_VALIDATED';


export const initialState = {
  repeatPrescriptionCourses: [],
  hasLoaded: false,
  hasErrored: false,
  validated: false,
  isValid: false,
};
