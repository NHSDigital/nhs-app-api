import isEmpty from 'lodash/fp/isEmpty';
import isString from 'lodash/fp/isString';
import values from 'lodash/fp/values';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import {
  ADD_API_ERROR,
  SET_ROUTE_PATH,
  DISABLE_API_ERROR,
  CLEAR_ALL_API_ERRORS,
  SET_CONNECTION_PROBLEM,
} from './mutation-types';

const extractPath = (route) => {
  if (isString(route)) return route;

  const paramValues = values(route.params);
  if (isEmpty(paramValues)) return route.path;
  return isEmpty(paramValues)
    ? route.path
    : paramValues.reduce((aggregate, next) => aggregate.replace(`/${next}`, ''), route.path);
};

export default {
  addApiError({ commit }, error) {
    commit(ADD_API_ERROR, error);
  },
  setRoutePath({ commit }, route) {
    commit(CLEAR_ALL_API_ERRORS);
    commit(SET_ROUTE_PATH, extractPath(route));
  },
  disableApiError({ commit }) {
    commit(DISABLE_API_ERROR);
  },
  clearAllApiErrors({ commit }) {
    commit(CLEAR_ALL_API_ERRORS);
  },
  setConnectionProblem({ commit }, hasConnectionProblem) {
    commit(SET_CONNECTION_PROBLEM, hasConnectionProblem);

    if (hasConnectionProblem) {
      EventBus.$emit(UPDATE_HEADER, 'noConnection.header');
      EventBus.$emit(UPDATE_TITLE, 'noConnection.header');
    }
  },
};
