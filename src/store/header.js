export const UPDATE_HEADER_TEXT = 'UPDATE_HEADER_TEXT';
const INIT_HEADER = 'INIT_HEADER';

export const actions = {
  updateHeaderText({ commit }, header) {
    commit(UPDATE_HEADER_TEXT, header);
  },
  initHeader({ commit }) {
    commit(INIT_HEADER);
  },
};

/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [UPDATE_HEADER_TEXT](state, header) {
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.updateHeaderText(header);
    } else {
      state.headerText = header;
    }
  },
  [INIT_HEADER](state) {
    state = {
      headerText: '',
    };
  },
};

export const state = () => ({
  headerText: '',
});
