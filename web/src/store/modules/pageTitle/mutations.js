/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import NativeCallbacks from '@/services/native-app';
import { INIT_PAGE_TITLE, UPDATE_PAGE_TITLE } from './mutation-types';

export default {
  [UPDATE_PAGE_TITLE](state, pageTitle) {
    if (process.client && state.isNativeApp === true) {
      NativeCallbacks.updatePageTitle(pageTitle);
    } else {
      state.pageTitle = pageTitle;
    }
  },
  [INIT_PAGE_TITLE](state) {
    state = Object.assign({}, {
      pageTitle: '',
    });
  },
};
