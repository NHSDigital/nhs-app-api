import {
  LOADING_COMPLETE,
  IS_LOADING,
} from './mutation-types';

export default {
  [LOADING_COMPLETE](state) {
    state.isLoading = false;
  },
  [IS_LOADING](state) {
    state.isLoading = true;
  },
};
