import {
  INIT_ANALYTICS,
  CLEAR_ACTION,
  TRACK_ACTION,
} from './mutation-types';

const clear = (state) => {
  state.action = '';
  state.timestamp = '';
};
export default {
  [INIT_ANALYTICS](state) {
    clear(state);
  },
  [CLEAR_ACTION](state) {
    clear(state);
  },
  [TRACK_ACTION](state, action) {
    state.timestamp = Date.now();
    state.action = `${action.type}|${action.senderType}|${action.target}`;
  },
};
