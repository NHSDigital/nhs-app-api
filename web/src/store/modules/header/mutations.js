import {
  UPDATE_HEADER_TEXT,
  INIT_HEADER,
  TOGGLE_MINI_MENU,
  CLOSE_MINI_MENU,
  UPDATE_HEADER_CAPTION,
} from './mutation-types';

export default {
  [UPDATE_HEADER_TEXT](state, header) {
    state.headerText = header;
  },
  [UPDATE_HEADER_CAPTION](state, headerCaption) {
    state.headerCaption = headerCaption;
  },
  [INIT_HEADER](state) {
    state.headerText = '';
    state.headerCaption = '';
  },
  [TOGGLE_MINI_MENU](state) {
    state.miniMenuExpanded = !state.miniMenuExpanded;
  },
  [CLOSE_MINI_MENU](state) {
    state.miniMenuExpanded = false;
  },
};
