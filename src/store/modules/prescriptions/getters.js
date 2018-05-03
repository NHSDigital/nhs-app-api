import { assign } from 'lodash/fp';

export const coursesAndRepeatPrescriptions = state =>
  state.coursesAndRepeatPrescriptions.map((prescription) => {
    const result = assign({}, prescription);

    return result;
  });


export default {
  coursesAndRepeatPrescriptions,
};
