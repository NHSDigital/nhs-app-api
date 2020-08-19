export const REPEAT_PRESCRIPTION_COURSES_LOADED = 'REPEAT_PRESCRIPTION_COURSES_LOADED';
export const INIT_REPEAT_PRESCRIPTIONS = 'INIT_REPEAT_PRESCRIPTIONS';
export const SELECT_REPEAT_PRESCRIPTION = 'SELECT_REPEAT_PRESCRIPTION';
export const REPEAT_PRESCRIPTION_VALIDATED = 'REPEAT_PRESCRIPTION_VALIDATED';
export const REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO = 'REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO';
export const PARTIAL_ORDER_RESULT = 'PARTIAL_ORDER_RESULT';
export const PRESCRIPTION_ORDER_JOURNEY_COMPLETE = 'PRESCRIPTION_ORDER_JOURNEY_COMPLETE';
export const PRESCRIPTIONS_JOURNEY_COMPLETE = 'PRESCRIPTIONS_JOURNEY_COMPLETE';
export const PRESCRIPTIONS_JOURNEY_START = 'PRESCRIPTIONS_JOURNEY_START';
export const ADD_ERROR = 'ADD_ERROR';

export const initialState = () => ({
  courses: [],
  repeatPrescriptionCourses: [],
  partialOrderResult: null,
  specialRequest: null,
  hasLoaded: false,
  hasErrored: false,
  validated: false,
  isValid: false,
  selectedCoursesNoJs: [],
  submitted: false,
  orderInProgress: false,
  error: undefined,
});
