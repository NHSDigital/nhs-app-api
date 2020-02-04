import mapKeys from 'lodash/fp/mapKeys';
import { initialState, INIT, LOADSERVICES } from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();

    mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADSERVICES](state, urls) {
    state.knownServices = urls;
    state.isLoaded = true;
  },
};
