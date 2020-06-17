import {
  TOGGLE_MINI_MENU,
  CLOSE_MINI_MENU,
} from './mutation-types';

export default {
  [TOGGLE_MINI_MENU](state) {
    state.miniMenuExpanded = !state.miniMenuExpanded;
  },
  [CLOSE_MINI_MENU](state) {
    state.miniMenuExpanded = false;
  },
};
