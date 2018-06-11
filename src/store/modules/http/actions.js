import {
  IS_LOADING,
  LOADING_COMPLETE,
  INIT_HTTP,
} from './mutation-types';


export default {
  loadingCompleted({ commit }) {
    commit(LOADING_COMPLETE, true);
  },
  isLoading({ commit }) {
    commit(IS_LOADING, true);
    this.dispatch('session/updateLastCalledAt');
  },
  init({ commit }) {
    commit(INIT_HTTP);
  },
};
