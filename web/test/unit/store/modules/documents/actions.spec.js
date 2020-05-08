import sanitize from 'sanitize-filename';

import NativeCallbacks from '@/services/native-app';
import actions from '@/store/modules/documents/actions';
import { SET_SELECTED_DOCUMENT_INFO, LOADED_DOCUMENT } from '@/store/modules/documents/mutation-types';

jest.mock('sanitize-filename');
jest.mock('@/services/native-app');

sanitize.mockImplementation((f, o) => f.replace('/', o.replacement));

let commit;
let state;
let payLoad;
let dispatch;
let $http;

const postResponse = {
  content: 'abcde1237dbe7c8a82173718dbad8ebd1b23',
  isValidFile: true,
  isViewable: true,
  isDownloadable: true,
  type: '.responsetype',
};

const mockStoreState = ({
  fileName,
  fileExtension,
  mimeType,
  isNative,
}) => {
  commit = jest.fn();

  state = {
    currentDocument: {
      type: '.type',
      name: 'x-ray-results',
    },
  };

  payLoad = {
    documentIdentifier: 'document-id-for-test',
    updateMetaData: false,
    fileName,
    fileExtension,
    mimeType,
    isNative,
  };

  dispatch = jest.fn();

  $http = {
    postV1DocumentsByDocumentidentifier: jest
      .fn()
      .mockImplementation(() => Promise.resolve(postResponse)),
    postV1DocumentsByDocumentidentifierDownload: jest
      .fn()
      .mockImplementation(() => Promise.resolve(postResponse)),
  };

  actions.app = {
    get $http() {
      return $http;
    },
  };
  actions.dispatch = dispatch;
};

const buildFileDetails = isNative => ({
  fileName: 'test / file',
  fileExtension: 'pdf',
  mimeType: 'application/pdf',
  isNative,
});

describe('documents store actions', () => {
  beforeEach(() => mockStoreState(buildFileDetails()));

  describe('loadDocument', () => {
    it('will call the postV1DocumentsByDocumentidentifier endpoint with the ' +
       'current document id, name and type', async () => {
      await actions.loadDocument({ commit, state }, payLoad);

      expect($http.postV1DocumentsByDocumentidentifier).toBeCalledWith({
        documentIdentifier: payLoad.documentIdentifier,
        getPatientDocumentRequest: {
          type: state.currentDocument.type,
          name: state.currentDocument.name,
        },
      });
    });

    it('will commit LOADED_DOCUMENT with the response content', async () => {
      await actions.loadDocument({ commit, state }, payLoad);

      expect(commit).toHaveBeenCalledWith(LOADED_DOCUMENT, postResponse.content);
    });

    it('will not commit SET_SELECTED_DOCUMENT_INFO when updateMetaData is false', async () => {
      const { isValidFile, isViewable, isDownloadable, type } = postResponse;

      await actions.loadDocument({ commit, state }, payLoad);

      expect(commit).not.toHaveBeenCalledWith(SET_SELECTED_DOCUMENT_INFO, {
        ...state.currentDocument,
        isValidFile,
        isViewable,
        isDownloadable,
        type,
      });
    });

    it('will commit SET_SELECTED_DOCUMENT_INFO when updateMetaData is true', async () => {
      const { isValidFile, isViewable, isDownloadable, type } = postResponse;
      payLoad.updateMetaData = true;

      await actions.loadDocument({ commit, state }, payLoad);

      expect(commit).toHaveBeenCalledWith(SET_SELECTED_DOCUMENT_INFO, {
        ...state.currentDocument,
        isValidFile,
        isViewable,
        isDownloadable,
        type,
      });
    });
  });

  describe('downloadDocument', () => {
    let createElement;
    let link;
    let appendChild;

    beforeEach(() => {
      window.URL.createObjectURL = jest.fn(() => 'mock blob href');

      link = { click: jest.fn() };

      createElement = jest.fn(() => link);
      appendChild = jest.fn();

      document.createElement = createElement;
      document.body.appendChild = appendChild;
      document.body.removeChild = jest.fn();
    });

    it('will call the postV1DocumentsByDocumentidentifier endpoint with the ' +
       'current document id, name and type', async () => {
      await actions.downloadDocument({ commit, state }, payLoad);

      expect($http.postV1DocumentsByDocumentidentifierDownload).toBeCalledWith({
        documentIdentifier: payLoad.documentIdentifier,
        getPatientDocumentRequest: {
          type: '.type',
          name: 'test / file',
        },
      });
    });

    it('will create a download link', async () => {
      await actions.downloadDocument({ commit, state }, payLoad);

      expect(createElement).toHaveBeenCalledWith('a');
      expect(appendChild).toHaveBeenCalledWith(link);
    });

    it('will set link href to object URL', async () => {
      await actions.downloadDocument({ commit, state }, payLoad);

      expect(link.href).toEqual('mock blob href');
    });

    it('will click the download link', async () => {
      await actions.downloadDocument({ commit, state }, payLoad);

      expect(link.click).toHaveBeenCalled();
    });

    it('will sanitize the filename for download', async () => {
      await actions.downloadDocument({ commit, state }, payLoad);

      expect(sanitize).toBeCalledWith('test / file.pdf', { replacement: '_' });
    });

    it('will use sanitized filename in download link', async () => {
      await actions.downloadDocument({ commit, state }, payLoad);

      expect(link.download).toEqual('test _ file.pdf');
    });

    describe('on a native device', () => {
      let startDownload;

      beforeEach(() => {
        startDownload = jest.fn();

        NativeCallbacks.startDownload.mockImplementation(startDownload);
        mockStoreState(buildFileDetails(true));
      });

      it('will call native callback', async () => {
        await actions.downloadDocument({ commit, state }, payLoad);

        expect(startDownload).toHaveBeenCalled();
      });

      it('will pass document details to native callback', async () => {
        await actions.downloadDocument({ commit, state }, payLoad);

        expect(startDownload).toHaveBeenCalledWith(
          'data:application/pdf;base64,W29iamVjdCBPYmplY3Rd',
          'test _ file.pdf',
          'application/pdf',
        );
      });
    });
  });
});
