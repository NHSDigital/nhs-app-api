import {
  ADD,
  CLEAR,
  HAS_BEEN_SHOWN,
  SHOW,
  INIT,
} from './mutation-types';

export default {
  addSuccess({ commit }, flashMessage) {
    commit(ADD, { message: flashMessage, type: 'success' });
  },
  addWarning({ commit }, flashMessage) {
    commit(ADD, { message: flashMessage, type: 'warning' });
  },
  addError({ commit }, flashMessage) {
    commit(ADD, { message: flashMessage, type: 'error' });
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
  show({ commit }) {
    commit(SHOW);
  },
};
