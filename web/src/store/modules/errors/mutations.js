/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import ErrorSettings from '@/components/errors/Settings';
import {
  ADD_API_ERROR,
  SET_ROUTE_PATH,
  DISABLE_API_ERROR,
  CLEAR_ALL_API_ERRORS,
  SET_CONNECTION_PROBLEM,
  ADD_ERROR,
} from './mutation-types';

export default {
  [ADD_API_ERROR](state, error) {
    let statusCode;

    if (error == null) {
      return;
    }

    if (error.response) {
      statusCode = error.response.status;
    } else {
      statusCode = 500;
      state.hasConnectionProblem = true;
    }
    const apiError = {
      status: statusCode,
      message: error.message,
    };

    state.apiErrors.push(apiError);
  },
  [ADD_ERROR](state, error) {
    if (error === null) {
      return;
    }
    state.errors.push(error);
  },
  [SET_ROUTE_PATH](state, route) {
    state.pageSettings = ErrorSettings.forPage(route);
    state.routePath = route;
  },
  [DISABLE_API_ERROR](state) {
    state.showApiError = false;
  },
  [CLEAR_ALL_API_ERRORS](state) {
    state.apiErrors = [];
    state.showApiError = true;
    state.routePath = '';
    state.pageSettings = ErrorSettings.default;
  },
  [SET_CONNECTION_PROBLEM](state, hasConnectionProblem) {
    state.hasConnectionProblem = hasConnectionProblem;
  },
};
