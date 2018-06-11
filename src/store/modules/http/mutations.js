/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  IS_LOADING,
  LOADING_COMPLETE,
  INIT_HTTP,
} from './mutation-types';

export default {
  [LOADING_COMPLETE](state) {
    state.isLoading = false;
  },
  [IS_LOADING](state) {
    state.isLoading = true;
  },
  [INIT_HTTP](state) {
    state = {
      isLoading: false,
    };
  },
};
