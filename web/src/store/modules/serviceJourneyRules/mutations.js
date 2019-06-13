import mapKeys from 'lodash/fp/mapKeys';
import { initialState, INIT, SET_RULES } from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();

    mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_RULES](state, rules) {
    state.rules = rules;
    state.isLoaded = true;
  },
};
