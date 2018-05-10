import { assign, map, mapKeys } from 'lodash/fp';
import { REPEAT_PRESCRIPTION_COURSES_LOADED } from './mutation-types';

export default {
  [REPEAT_PRESCRIPTION_COURSES_LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.repeatPrescriptionCourses = map((course) => {
      const result = assign({}, course);
      return result;
    })(state.repeatPrescriptionCourses);

    state.REPEAT_PRESCRIPTION_COURSES_LOADED = true;
  },
};
