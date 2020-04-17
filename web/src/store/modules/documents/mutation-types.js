export const INIT = 'INIT';
export const CLEAR = 'CLEAR';
export const SET_VALID_FILE = 'SET_VALID_FILE';
export const SET_IS_VIEWABLE = 'SET_IS_VIEWABLE';
export const SET_IS_DOWNLOADABLE = 'SET_IS_DOWNLOADABLE';
export const SET_FILE_TYPE = 'SET_FILE_TYPE';
export const LOADED_DOCUMENT = 'LOADED_DOCUMENT';
export const SET_SELECTED_DOCUMENT_INFO = 'SET_SELECTED_DOCUMENT_INFO';
export const initialState = () => ({
  currentDocument: {
    isViewable: true,
    isDownloadable: true,
  },
  documentConsultationsWithComments: [],
});
