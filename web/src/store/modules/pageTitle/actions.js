import { INIT_PAGE_TITLE, UPDATE_PAGE_TITLE } from './mutation-types';

export default {
  updatePageTitle({ commit }, pageTitle) {
    commit(UPDATE_PAGE_TITLE, pageTitle);
  },
  init({ commit }) {
    commit(INIT_PAGE_TITLE);
  },
};
