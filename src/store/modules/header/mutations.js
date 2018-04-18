import { SET_TITLE } from './mutation-types';

export default {
  [SET_TITLE](state, title) {
    state.title = title;
    return state;
  },
};
