import { assign } from 'lodash/fp';

export default {
  prescriptionCourses(state) {
    return state.prescriptionCourses.map((prescription) => {
      const result = assign({}, prescription);
      return result;
    });
  },
};
