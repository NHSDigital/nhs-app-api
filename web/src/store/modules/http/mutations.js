/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  IS_LOADING,
  IS_LOADING_EXTERNAL_SITE,
  LOADING_COMPLETE,
  ADD_CANCEL_REQUEST_HANDLER,
  CANCEL_REQUESTS,
  INIT_HTTP,
} from './mutation-types';

export default {
  [LOADING_COMPLETE](state, url) {
    state.loadingUrls = state.loadingUrls.filter(loading => loading !== url);
  },
  [IS_LOADING](state, url) {
    state.loadingUrls.push(url);
  },
  [IS_LOADING_EXTERNAL_SITE](state) {
    state.isLoadingExternalSite = true;
  },
  [ADD_CANCEL_REQUEST_HANDLER](state, handler) {
    state.cancelRequestHandlers.push(handler);
  },
  [CANCEL_REQUESTS](state) {
    let index = 0;
    for (index; index < state.cancelRequestHandlers.length; index += 1) {
      const handler = state.cancelRequestHandlers[index];
      if (typeof handler === 'function') handler();
    }
    state.cancelRequestHandlers = [];
    state.loadingUrls = [];
  },
  [INIT_HTTP](state) {
    state = {
      loadingUrls: [],
    };
  },
};
