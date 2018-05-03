import { assign, map, mapKeys } from 'lodash/fp';
import { PRESCRIPTIONS_LOADED } from './mutation-types';

export default {
  [PRESCRIPTIONS_LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.coursesAndRepeatPrescriptions = map((prescription) => {
      const result = assign({}, prescription);
      return result;
    })(state.coursesAndRepeatPrescriptions);

    state.PRESCRIPTIONS_LOADED = true;
  },
};
