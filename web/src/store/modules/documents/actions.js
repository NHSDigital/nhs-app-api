import sanitize from 'sanitize-filename';

import {
  INIT,
  CLEAR,
  LOADED_DOCUMENT,
  SET_SELECTED_DOCUMENT_INFO,
} from './mutation-types';
import NativeCallbacks from '@/services/native-app';

const sanitizeFilename = sanitize;

const SANITIZE_FILENAME_OPTIONS = { replacement: '_' };

export default {
  init({ commit }) {
    commit(INIT);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  async loadDocument({ commit, state }, { documentIdentifier, updateMetaData = false } = {}) {
    const response = await this.app.$http.postV1DocumentsByDocumentidentifier({
      documentIdentifier,
      getPatientDocumentRequest: {
        type: state.currentDocument.type,
        name: state.currentDocument.name,
      },
    }) || {};
    const { content, isViewable, isDownloadable, type } = response || {};

    commit(LOADED_DOCUMENT, content);
    if (updateMetaData) {
      commit(SET_SELECTED_DOCUMENT_INFO, {
        ...state.currentDocument,
        isValidFile: isViewable || isDownloadable,
        isViewable,
        isDownloadable,
        type,
      });
    }
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
    const fullFileName = sanitizeFilename(`${fileName}.${fileExtension}`, SANITIZE_FILENAME_OPTIONS);
    /*
      IE does not support the File constructor or createObjectURL so need
      to use msSaveOrOpenBlob. Trident is used to match IE11.

      All other relevant browsers support File but do not seem to
      download if the Blob constructor is used.
    */
    if (userAgent.match(/Trident/i)) {
      const blob = new Blob([response], fullFileName, { type: mimeType });

      window.navigator.msSaveOrOpenBlob(blob, fullFileName);

      return;
    }

    // Chrome for iOS
    if (userAgent.match(/CriOS/i)) {
      const reader = new FileReader();
      const blob = new File([response], fullFileName, { type: mimeType });

      reader.onload = () => {
        window.location.href = reader.result;
      };
      reader.readAsDataURL(blob);

      return;
    }

    let blob;

    // Edge does not support the File constructor
    if (userAgent.match(/Edge/i)) {
      blob = new Blob([response], fullFileName, { type: mimeType });
    } else {
      blob = new File([response], fullFileName, { type: mimeType });
    }

    if (isNative) {
      const fileReader = new FileReader();

      fileReader.readAsDataURL(blob);

      await new Promise((resolve, reject) => {
        fileReader.onloadend = () => {
          const base64data = fileReader.result;

          NativeCallbacks.startDownload(base64data, fullFileName, mimeType);

          resolve();
        };

        fileReader.onerror = reject;
        fileReader.onabort = reject;
      });

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
