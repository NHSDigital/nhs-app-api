import mapKeys from 'lodash/fp/mapKeys';
import { mutationNames } from './constants';
import { initialState } from './mutation-types';

const { INIT, SET_RULES } = mutationNames;

export default {
  [INIT](state) {
    const blank = initialState();

    mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_RULES](state, rules) {
    state.rules = rules.journeys;
    state.isLoaded = true;
  },
};
