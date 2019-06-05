/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { SHOW_MODAL, HIDE_MODAL, DESTROY_MODAL } from './mutation-types';

export default {
  [SHOW_MODAL](state, config) {
    state.config = config;
    state.config.visible = true;
  },
  [HIDE_MODAL]() {
  },
  [DESTROY_MODAL](state) {
    state.config = {
      visible: false,
    };
  },
};
