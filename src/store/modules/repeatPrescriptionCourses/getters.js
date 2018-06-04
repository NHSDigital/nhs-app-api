import { assign } from 'lodash/fp';

export default {
  /* eslint-disable no-shadow */
  repeatPrescriptionCourses(state) {
    state.repeatPrescriptionCourses.map((course) => {
      const result = assign({}, course);
      return result;
    });
  },
  selectedPrescriptions(state) {
    const selectedCourses = [];
    state.repeatPrescriptionCourses.forEach((course) => {
      if (course.selected) {
        selectedCourses.push(course);
      }
    });
    return selectedCourses;
  },
  isValid(state) {
    const selectedCourses = [];
    state.repeatPrescriptionCourses.forEach((course) => {
      if (course.selected) {
        selectedCourses.push(course);
      }
    });
    return selectedCourses.length > 0;
  },
};
