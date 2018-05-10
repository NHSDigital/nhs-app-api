import { assign } from 'lodash/fp';

export const repeatPrescriptionCourses = state =>
  state.repeatPrescriptionCourses.map((course) => {
    const result = assign({}, course);

    return result;
  });


export default {
  repeatPrescriptionCourses,
};
