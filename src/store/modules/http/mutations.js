/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  CLEAR_API_ERROR_RESPONSE,
  IS_LOADING,
  LOADING_COMPLETE,
  SET_API_ERROR_RESPONSE,
  INIT_HTTP,
} from './mutation-types';

export default {
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
