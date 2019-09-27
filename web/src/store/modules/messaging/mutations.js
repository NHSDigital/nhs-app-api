import mapKeys from 'lodash/fp/mapKeys';
import {
  initialState,
  INIT,
  LOADED,
} from './mutation-types';

export default {
  [LOADED](state, data) {
    state.unreadMessages = data;
    state.hasLoaded = true;
  },
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
};
