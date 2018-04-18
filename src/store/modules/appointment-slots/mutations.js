import { mapKeys } from 'lodash/fp';
import { SLOTS_LOADED } from './mutation-types';

export default {
  [SLOTS_LOADED](state, slots) {
    mapKeys((key) => {
      state[key] = slots[key];
    })(slots);
  },
};
