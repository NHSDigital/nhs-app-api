export const IS_LOADING = 'IS_LOADING';
export const LOADING_COMPLETE = 'LOADING_COMPLETE';
const INIT_HTTP = 'INIT_HTTP';

export const state = () => ({
  isLoading: false,
});

export const actions = {
  loadingCompleted({ commit }) {
    commit(LOADING_COMPLETE, true);
  },
  isLoading({ commit }) {
    commit(IS_LOADING, true);
  },
  init({ commit }) {
    commit(INIT_HTTP);
  },
};
/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [LOADING_COMPLETE](state) {
    state.isLoading = false;
  },
  [IS_LOADING](state) {
    state.isLoading = true;
  },
  [INIT_HTTP](state) {
    state = {
      isLoading: false,
    };
  },
};
