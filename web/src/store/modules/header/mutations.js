/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import NativeCallbacks from '@/services/native-app';
import {
  UPDATE_HEADER_TEXT,
  INIT_HEADER,
  TOGGLE_MINI_MENU,
  CLOSE_MINI_MENU,
} from './mutation-types';

export default {
  [UPDATE_HEADER_TEXT](state, header) {
    if (process.client && window.nativeApp) {
      NativeCallbacks.updateHeaderText(header);
    }
    state.headerText = header;
  },
  [INIT_HEADER](state) {
    state = Object.assign({}, {
      headerText: '',
    });
  },
  [TOGGLE_MINI_MENU](state) {
    state.miniMenuExpanded = !state.miniMenuExpanded;
  },
  [CLOSE_MINI_MENU](state) {
    state.miniMenuExpanded = false;
  },
};
