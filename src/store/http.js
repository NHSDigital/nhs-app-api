export const IS_LOADING = 'IS_LOADING';
export const LOADING_COMPLETE = 'LOADING_COMPLETE';
export const SET_API_ERROR_RESPONSE = 'SET_API_ERROR_RESPONSE';
export const CLEAR_API_ERROR_RESPONSE = 'CLEAR_API_ERROR_RESPONSE';
const INIT_HTTP = 'INIT_HTTP';

export const state = () => ({
  isLoading: false,
  apiErrorResponse: null,
});

export const actions = {
  loadingCompleted({ commit }) {
    commit(LOADING_COMPLETE, true);
  },
  isLoading({ commit }) {
    commit(IS_LOADING, true);
  },
  setApiErrorResponse ({ commit }, error ) {
    commit(SET_API_ERROR_RESPONSE, error);
  },
  clearApiErrorResponse({ commit }) {
    commit(CLEAR_API_ERROR_RESPONSE, null);
  },
  init({ commit }) {
    commit(INIT_HTTP);
  },
};
/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [LOADING_COMPLETE](state) {
    state.isLoading = false;
  },
  [IS_LOADING](state) {
    state.isLoading = true;
  },
  [SET_API_ERROR_RESPONSE](state, error) {
    if (error == null) {
      state.apiErrorResponse = null;
      return;
    }

    const statusCode = (error.response) ? error.response.status : 500;
    const apiError = {
      status: statusCode,
      message: error.message,
    };

    state.apiErrorResponse = apiError;
  },
  [CLEAR_API_ERROR_RESPONSE](state) {
    state.apiErrorResponse = null;
  },
  [INIT_HTTP](state) {
    state = {
      isLoading: false,
    };
  },
};
