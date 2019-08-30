/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { INIT_PAGE_TITLE, UPDATE_PAGE_TITLE } from './mutation-types';

export default {
  [UPDATE_PAGE_TITLE](state, pageTitle) {
    state.pageTitle = pageTitle;
  },
  [INIT_PAGE_TITLE](state) {
    state = Object.assign({}, {
      pageTitle: '',
    });
  },
};
