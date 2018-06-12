/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
/* eslint-disable object-shorthand */
/* eslint-disable func-names */
import {
  ADD_API_ERROR,
  SET_API_ERROR_BUTTON_PATH,
  SET_SHOWING_API_ERROR_CONDITION,
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
  [SET_SHOWING_API_ERROR_CONDITION](state, condition) {
    state.showingApiErrorCondition = condition;
  },
  [CLEAR_ALL_API_ERRORS](state) {
    state.apiErrors = [];
    state.showingApiErrorCondition = function (status) {
      return (status >= 500 || status === 403);
    };
    state.apiErrorButtonPath = '';
  },
  [SET_CONNECTION_PROBLEM](state, hasConnectionProblem) {
    state.hasConnectionProblem = hasConnectionProblem;
  },
};
