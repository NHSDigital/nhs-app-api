import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  LOADED,
  SET_SUMMARIES,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_SUMMARIES](state, data) {
    state.messageSummaries = data || [];
  },
  [LOADED](state, loaded) {
    state.loaded = !!loaded;
  },
};
