import { assign, map, mapKeys } from 'lodash/fp';
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  INIT_REPEAT_PRESCRIPTIONS,
  REPEAT_PRESCRIPTION_VALIDATED,
  SELECT_REPEAT_PRESCRIPTION,
  initialState,
} from './mutation-types';


export default {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [REPEAT_PRESCRIPTION_COURSES_LOADED](state, data) {
    const { response: courses } = data;
    mapKeys((key) => {
      state[key] = courses[key];
    })(courses);

    state.repeatPrescriptionCourses = map((course) => {
      const result = assign({}, course);
      result.selected = false;
      return result;
    })(state.courses);
    state.loaded = true;
  },
  [INIT_REPEAT_PRESCRIPTIONS](state) {
    mapKeys((key) => {
      state[key] = initialState[key];
    })(initialState);
  },
  [REPEAT_PRESCRIPTION_VALIDATED](state, validationObject) {
    if (validationObject.submitted) {
      state.validated = true;
    }
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
};

