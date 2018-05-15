import { assign } from 'lodash/fp';

export const prescriptionCourses = state =>
  state.prescriptionCourses.map((prescription) => {
    const result = assign({}, prescription);

    return result;
  });


export default {
  prescriptionCourses,
};
