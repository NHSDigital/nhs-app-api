export const REPEAT_PRESCRIPTION_COURSES_LOADED =
  'REPEAT_PRESCRIPTION_COURSES_LOADED';
export const INIT_REPEAT_PRESCRIPTIONS = 'INIT_REPEAT_PRESCRIPTIONS';
export const SELECT_REPEAT_PRESCRIPTION = 'SELECT_REPEAT_PRESCRIPTION';
export const REPEAT_PRESCRIPTION_VALIDATED = 'REPEAT_PRESCRIPTION_VALIDATED';
export const REPEAT_PRESCRIPTION_ORDER_SUCCESS = 'REPEAT_PRESCRIPTION_ORDER_SUCCESS';


export const initialState = {
  courses: [],
  loaded: false,
  errored: false,
  repeatPrescriptionCourses: [],
  justOrderedARepeatPrescription: false,
  hasLoaded: false,
  hasErrored: false,
  validated: false,
  valid: false,
};
