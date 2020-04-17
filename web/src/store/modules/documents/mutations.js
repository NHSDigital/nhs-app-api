/* eslint-disable no-param-reassign */
import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  CLEAR,
  LOADED_DOCUMENT,
  SET_SELECTED_DOCUMENT_INFO,
  SET_VALID_FILE,
  SET_IS_VIEWABLE,
  SET_IS_DOWNLOADABLE,
  SET_FILE_TYPE,
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
  [SET_VALID_FILE](state, isValidFile) {
    state.currentDocument.isValidFile = isValidFile;
  },
  [SET_IS_VIEWABLE](state, isViewable) {
    state.currentDocument.isViewable = isViewable;
  },
  [SET_IS_DOWNLOADABLE](state, isDownloadable) {
    state.currentDocument.isDownloadable = isDownloadable;
  },
  [SET_FILE_TYPE](state, type) {
    state.currentDocument.type = type;
  },
};
