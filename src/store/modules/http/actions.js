import {
  CLEAR_API_ERROR_RESPONSE,
  IS_LOADING,
  LOADING_COMPLETE,
  SET_API_ERROR_RESPONSE,
  INIT_HTTP,
} from './mutation-types';


export default {
  loadingCompleted({ commit }) {
    commit(LOADING_COMPLETE, true);
  },
  isLoading({ commit }) {
    commit(IS_LOADING, true);
  },
  setApiErrorResponse({ commit }, error) {
    commit(SET_API_ERROR_RESPONSE, error);
  },
  clearApiErrorResponse({ commit }) {
    commit(CLEAR_API_ERROR_RESPONSE, null);
  },
  init({ commit }) {
    commit(INIT_HTTP);
  },
};
