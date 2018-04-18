import {
  UPDATE_HEADER_TEXT,
} from './mutation-types';

export default {
  [UPDATE_HEADER_TEXT](state, header) {
    if (typeof window.nativeApp !== 'undefined') {
      window.nativeApp.updateHeaderText(header);
    } else {
      state.headerText = header;
    }
  },
};
