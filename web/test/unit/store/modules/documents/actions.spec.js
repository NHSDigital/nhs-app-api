import actions from '@/store/modules/documents/actions';
import { SET_SELECTED_DOCUMENT_INFO, LOADED_DOCUMENT } from '@/store/modules/documents/mutation-types';

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

describe('documents store actions', () => {
  beforeEach(() => {
    commit = jest.fn();
    state = {
      currentDocument: {
        type: '.type',
        name: 'x-ray-results',
      },
    };
    payLoad = { documentIdentifier: 'document-id-for-test', updateMetaData: false };
    dispatch = jest.fn();
    $http = {
      postV1DocumentsByDocumentidentifier: jest
        .fn()
        .mockImplementation(() => Promise.resolve(postResponse)),
    };
    actions.app = {
      get $http() {
        return $http;
      },
    };
    actions.dispatch = dispatch;
  });

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
});
