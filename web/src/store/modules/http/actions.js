import {
  IS_LOADING,
  IS_LOADING_EXTERNAL_SITE,
  LOADING_COMPLETE,
  INIT_HTTP,
  ADD_CANCEL_REQUEST_HANDLER,
  CANCEL_REQUESTS,
} from './mutation-types';


export default {
  loadingCompleted({ commit }, url) {
    if (!url) {
      this.dispatch('log/onError', 'url not specified in loadingCompleted');
    }

    commit(LOADING_COMPLETE, url);
  },
  isLoadingExternalSite({ commit }) {
    commit(IS_LOADING_EXTERNAL_SITE);
  },
  isLoading({ commit }, url) {
    if (!url) {
      this.dispatch('log/onError', 'url not specified in isLoading');
    }

    commit(IS_LOADING, url);
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
