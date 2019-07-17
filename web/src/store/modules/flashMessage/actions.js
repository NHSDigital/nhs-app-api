import {
  ADD,
  CLEAR,
  HAS_BEEN_SHOWN,
  VALIDATE,
  INIT,
} from './mutation-types';

export default {
  addSuccess({ commit }, flashMessage) {
    commit(ADD, { message: flashMessage, type: 'success' });
  },
  addWarning({ commit }, flashMessage) {
    commit(ADD, { message: flashMessage, type: 'warning' });
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  init({ commit }) {
    commit(INIT);
  },
  hasBeenShown({ commit }) {
    commit(HAS_BEEN_SHOWN);
  },
  validate({ commit }, routeName) {
    commit(VALIDATE, routeName);
  },
};
