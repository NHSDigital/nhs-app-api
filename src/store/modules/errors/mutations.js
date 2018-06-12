/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  ADD_API_ERROR,
  SET_API_ERROR_BUTTON_PATH,
  DISABLE_API_ERROR,
  CLEAR_ALL_API_ERRORS,
  SET_CONNECTION_PROBLEM,
} from './mutation-types';

export default {
  [ADD_API_ERROR](state, error) {
    if (error == null) {
      return;
    }

    const statusCode = (error.response) ? error.response.status : 500;
    const apiError = {
      status: statusCode,
      message: error.message,
    };

    state.apiErrors.push(apiError);
  },
  [SET_API_ERROR_BUTTON_PATH](state, path) {
    state.apiErrorButtonPath = path;
  },
  [DISABLE_API_ERROR](state) {
    state.showApiError = false;
  },
  [CLEAR_ALL_API_ERRORS](state) {
    state.apiErrors = [];
    state.showApiError = true;
    state.apiErrorButtonPath = '';
  },
  [SET_CONNECTION_PROBLEM](state, hasConnectionProblem) {
    state.hasConnectionProblem = hasConnectionProblem;
  },
};
