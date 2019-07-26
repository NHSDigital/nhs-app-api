/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  ADD,
  CLEAR,
  INIT,
  SHOW,
  HAS_BEEN_SHOWN,
} from './mutation-types';

const clear = (state) => {
  state.message = '';
  state.hasBeenShown = false;
  state.show = false;
  state.type = 'success';
  state.key = '';
};
export default {
  [ADD](state, flashMessage) {
    state.message = flashMessage.message;
    state.type = flashMessage.type;
  },
  [CLEAR](state) {
    if (state.hasBeenShown) {
      clear(state);
    }
  },
  [HAS_BEEN_SHOWN](state) {
    state.hasBeenShown = true;
  },
  [INIT](state) {
    clear(state);
  },
  [SHOW](state) {
    state.show = state.message !== '';
  },
};
