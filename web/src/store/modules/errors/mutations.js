import ErrorSettings from '@/components/errors/Settings';
import {
  ADD_API_ERROR,
  CLEAR_ALL_API_ERRORS,
  DISABLE_API_ERROR,
  SET_CONNECTION_PROBLEM,
  SET_ROUTE_PATH,
  handledErrors,
  standardErrors,
} from './mutation-types';

const allErrors = standardErrors.concat(handledErrors);

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
      ({ errorCode, serviceDeskReference } = data || {});
    } else {
      statusCode = 500;
      state.hasConnectionProblem = true;
    }

    if (statusCode >= 500 || allErrors.indexOf(statusCode) !== -1) {
      const apiError = {
        status: statusCode,
        error: errorCode,
        message: error.message,
        serviceDeskReference: serviceDeskReference || '',
      };

      state.apiErrors.push(apiError);
    }
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
  },
  [SET_CONNECTION_PROBLEM](state, hasConnectionProblem) {
    state.hasConnectionProblem = hasConnectionProblem;
  },
};
