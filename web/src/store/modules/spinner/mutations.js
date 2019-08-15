import { PREVENT } from './mutation-types';

export default {
  [PREVENT](state, prevent) {
    state.prevent = prevent;
  },
};
