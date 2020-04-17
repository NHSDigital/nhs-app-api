import {
  INIT,
  CLEAR,
  LOADED_DOCUMENT,
  SET_SELECTED_DOCUMENT_INFO,
  SET_VALID_FILE,
  SET_IS_VIEWABLE,
  SET_IS_DOWNLOADABLE,
  SET_FILE_TYPE,
} from './mutation-types';
import NativeCallbacks from '@/services/native-app';

export default {
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  async loadDocument({ commit, state }, documentIdentifier) {
    const response = await this.app.$http.postV1DocumentsByDocumentidentifier({
      documentIdentifier,
      getPatientDocumentRequest: {
        type: state.currentDocument.type,
        name: state.currentDocument.name,
      },
    }) || {};
    const { content, isViewable, isDownloadable, type } = response || {};

    commit(LOADED_DOCUMENT, content);
    commit(SET_VALID_FILE, isViewable || isDownloadable);
    commit(SET_FILE_TYPE, type);
    commit(SET_IS_VIEWABLE, isViewable);
    commit(SET_IS_DOWNLOADABLE, isDownloadable);
  },
  async downloadDocument({ state }, { documentIdentifier, fileName,
    fileExtension, mimeType, isNative }) {
    const response = await this.app.$http.postV1DocumentsByDocumentidentifierDownload({
      documentIdentifier,
      getPatientDocumentRequest: {
        type: state.currentDocument.type,
        name: fileName,
      },
    });
    const { userAgent } = window.navigator;
    const fullFileName = `${fileName}.${fileExtension}`;
    let blob;

    /*
      Edge or IE do not support the File constructor
      IE does not support createObjectURL so need to use msSaveOrOpenBlob
      Trident is used to match IE11 and MSIE is used for IE < 11
      All other relevant browsers support File but do not seem to
      download if the Blob constructor is used.
    */
    if (userAgent.match(/Edge/i)) {
      blob = new Blob([response], fullFileName, { type: mimeType });
    }

    if (userAgent.match(/Trident/i)) {
      blob = new Blob([response], fullFileName, { type: mimeType });
      window.navigator.msSaveOrOpenBlob(blob, fullFileName);
      return;
    }

    if (userAgent.match(/CriOS/i)) {
      const reader = new FileReader();
      blob = new File([response], fullFileName, { type: mimeType });

      reader.onload = () => {
        window.location.href = reader.result;
      };
      reader.readAsDataURL(blob);
      return;
    }

    blob = new File([response], fullFileName, { type: mimeType });

    if (isNative) {
      const fileReader = new FileReader();

      fileReader.readAsDataURL(blob);
      fileReader.onloadend = () => {
        const base64data = fileReader.result;

        NativeCallbacks.startDownload(base64data, fullFileName, mimeType);
      };

      return;
    }

    const link = document.createElement('a');

    link.href = window.URL.createObjectURL(blob);
    link.download = fullFileName;
    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
  },
  setSelectedDocumentInfo({ commit }, selectedDocumentInfo) {
    commit(SET_SELECTED_DOCUMENT_INFO, selectedDocumentInfo);
  },
};
