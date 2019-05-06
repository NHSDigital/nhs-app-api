import {
  IS_LOADING,
  LOADING_COMPLETE,
  INIT_HTTP,
  ADD_CANCEL_REQUEST_HANDLER,
  CANCEL_REQUESTS,
} from './mutation-types';


export default {
  loadingCompleted({ commit }) {
    commit(LOADING_COMPLETE);
  },
  isLoading({ commit }) {
    commit(IS_LOADING);
    this.dispatch('session/updateLastCalledAt');
  },
  addCancelRequestHandler({ commit }, handler) {
    commit(ADD_CANCEL_REQUEST_HANDLER, handler);
  },
  cancelRequests({ commit }) {
    commit(CANCEL_REQUESTS);
  },
  init({ commit }) {
    commit(INIT_HTTP);
  },
};
