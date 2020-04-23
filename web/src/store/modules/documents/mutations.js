/* eslint-disable no-param-reassign */
import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  CLEAR,
  LOADED_DOCUMENT,
  SET_SELECTED_DOCUMENT_INFO,
  initialState,
} from './mutation-types';

const clearState = (state) => {
  state.currentDocument = {
    isViewable: true,
    isDownloadable: true,
  };
  state.documentConsultationsWithComments = [];
};

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [CLEAR](state) {
    clearState(state);
  },
  [LOADED_DOCUMENT](state, documentData) {
    state.currentDocument.data = documentData;
  },
  [SET_SELECTED_DOCUMENT_INFO](state, document) {
    state.currentDocument = document;
  },
};
