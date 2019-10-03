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
} from './mutation-types';

export default {
  [ADD_API_ERROR](state, error) {
    let statusCode;
    let errorCode;
    let serviceDeskReference;

    if (error == null) {
      return;
    }
    if (error.response) {
      const { status, data } = error.response;
      statusCode = status;
      if (error.response.data && error.response.data.serviceDeskReference) {
        this.serviceDeskReference = error.response.data.serviceDeskReference;
      }
      ({ errorCode } = data || {});
    } else {
      statusCode = 500;
      state.hasConnectionProblem = true;
    }

    const apiError = {
      status: statusCode,
      error: errorCode,
      message: error.message,
      serviceDeskReference: this.serviceDeskReference || '',
    };

    state.apiErrors.push(apiError);
  },
  [SET_ROUTE_PATH](state, route) {
    const routePath = route.replace(/\/$/, '');
    state.pageSettings = ErrorSettings.forPage(routePath);
    state.routePath = routePath;
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
