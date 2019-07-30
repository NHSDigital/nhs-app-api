import { assign, map, mapKeys } from 'lodash/fp';
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  INIT_REPEAT_PRESCRIPTIONS,
  REPEAT_PRESCRIPTION_VALIDATED,
  SELECT_REPEAT_PRESCRIPTION,
  REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO,
  PARTIAL_ORDER_RESULT,
  initialState,
} from './mutation-types';


export default {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [REPEAT_PRESCRIPTION_COURSES_LOADED](state, data) {
    const courses = assign({}, data);
    mapKeys((key) => {
      state[key] = courses[key];
    })(courses);
    state.repeatPrescriptionCourses = map((course) => {
      const result = assign({}, course);
      result.selected = false;
      return result;
    })(state.courses);
    state.hasLoaded = true;
  },
  [INIT_REPEAT_PRESCRIPTIONS](state) {
    const defaultValues = initialState();
    mapKeys((key) => {
      state[key] = defaultValues[key];
    })(state);
    state.selected = [];
  },
  [REPEAT_PRESCRIPTION_VALIDATED](state, validationObject) {
    state.validated = !!validationObject.submitted;
    state.isValid = validationObject.isValid;
  },
  [SELECT_REPEAT_PRESCRIPTION](state, id) {
    state.repeatPrescriptionCourses = state.repeatPrescriptionCourses.map((course) => {
      if (course.id === id) {
        course.selected = !course.selected;
      }
      return course;
    });
  },
  [REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO](state, repeatPrescriptionAdditionalInfo) {
    state.specialRequest = repeatPrescriptionAdditionalInfo.specialRequest;
  },
  [PARTIAL_ORDER_RESULT](state, data) {
    state.partialOrderResult = data;
  },
};

