import {
  ADD_API_ERROR,
  SET_API_ERROR_BUTTON_PATH,
  DISABLE_API_ERROR,
  CLEAR_ALL_API_ERRORS,
  SET_CONNECTION_PROBLEM,
} from './mutation-types';

export default {
  addApiError({ commit }, error) {
    commit(ADD_API_ERROR, error);
  },
  setApiErrorButtonPath({ commit }, path) {
    commit(SET_API_ERROR_BUTTON_PATH, path);
  },
  disableApiError({ commit }) {
    commit(DISABLE_API_ERROR);
  },
  clearAllApiErrors({ commit }) {
    commit(CLEAR_ALL_API_ERRORS);
  },
  setConnectionProblem({ commit }, hasConnectionProblem) {
    commit(SET_CONNECTION_PROBLEM, hasConnectionProblem);
  },
};
