import mapKeys from 'lodash/fp/mapKeys';
import {
  initialState,
  INIT,
  LOADED,
  SET_SENDER,
} from './mutation-types';

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [LOADED](state, data) {
    state.senderMessages = data;
  },
  [SET_SENDER](state, sender) {
    state.selectedSender = sender;
  },
};
