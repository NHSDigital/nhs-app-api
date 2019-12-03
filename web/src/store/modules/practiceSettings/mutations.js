import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  SET_IM1_MESSAGING_ENABLED,
  initialState,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [SET_IM1_MESSAGING_ENABLED](state, enabled) {
    state.im1MessagingEnabled = !!enabled;
  },
};
