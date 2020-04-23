export const INIT = 'INIT';
export const CLEAR = 'CLEAR';
export const LOADED_DOCUMENT = 'LOADED_DOCUMENT';
export const SET_SELECTED_DOCUMENT_INFO = 'SET_SELECTED_DOCUMENT_INFO';
export const initialState = () => ({
  currentDocument: {
    isViewable: true,
    isDownloadable: true,
  },
  documentConsultationsWithComments: [],
});
