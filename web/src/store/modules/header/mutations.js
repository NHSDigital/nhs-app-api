/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import NativeCallbacks from '@/services/native-app';
import { UPDATE_HEADER_TEXT, INIT_HEADER } from './mutation-types';

export default {
  [UPDATE_HEADER_TEXT](state, header) {
    if (process.client && window.nativeApp) {
      NativeCallbacks.updateHeaderText(header);
    } else {
      state.headerText = header;
    }
  },
  [INIT_HEADER](state) {
    state = Object.assign({}, {
      headerText: '',
    });
  },
};
