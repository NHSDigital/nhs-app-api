import { mount, createRouter, createStore } from '../../../helpers';
import DocumentInformation from '@/pages/my-record/documents/_id';
import { MY_RECORD_DOCUMENT_DETAIL, MYRECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { datePart } from '@/lib/utils';

jest.mock('@/lib/sessionStorage');
jest.mock('@/lib/utils');

let redirect;
const $router = createRouter();
const $route = {
  params: {
    id: 1,
  },
};

const newStore = ({ isDocumentsEnabled = true,
  document,
  documentConsultationsWithComments = [] } = {}) => (
  createStore({
    $env: {
      MY_RECORD_DOCUMENTS_ENABLED: isDocumentsEnabled,
    },
    state: {
      myRecord: {
        document,
        hasAcceptedTerms: true,
        documentConsultationsWithComments,
      },
      device: {
        isNativeApp: false,
      },
    },
  }));

const mountPage = ({ $store, data = () => ({ comments: [] }) }) =>
  mount(DocumentInformation, { $store, $router, $route, data });

describe('document view', () => {
  let $store;

  beforeEach(() => {
    redirect = jest.fn();
    hasAgreedToMedicalWarning.mockClear();
    hasAgreedToMedicalWarning.mockReturnValue(true);
    datePart.mockReturnValue('8 August 2019');
  });

  describe('asyncData', () => {
    it('will redirect to the my record page if the document environment variable is not enabled', async () => {
      // Arrange
      $store = newStore(false);
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect(redirect).toBeCalledWith(MYRECORD.path);
    });

    it('will redirect to the my record page if there is no document date', async () => {
      // Arrange
      const document = { date: { value: undefined } };
      $store = newStore(document);
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect(redirect).toBeCalledWith(MYRECORD.path);
    });

    it('will set the header and page title to the document name', async () => {
      // Arrange
      const document = { name: 'Document1', size: 1000000, date: { value: '2019-08-08T12:03:44+00:00' } };
      $store = newStore({ document });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'Document1');
    });

    it('will set the header to the document date if no name exists', async () => {
      // Arrange
      const document = { name: undefined, size: 1000000, date: { value: '2019-08-08T12:03:44+00:00' } };
      $store = newStore({ document });
      const page = mountPage({ $store });

      const dateString = 'translate_my_record.documents.documentPageSubtext 8 August 2019';

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', dateString);
    });
  });

  describe('methods', () => {
    it('will navigate to the view document page with the correct id in the path', () => {
      // Arrange
      const route = { name: MY_RECORD_DOCUMENT_DETAIL.name, params: { id: 1 } };
      const page = mountPage({ $store });

      // Act
      page.vm.navigateToView();

      // Assert
      expect($router.push).toHaveBeenCalledWith(route);
    });
    it('will convert map the download type correctly', async () => {
      // Arrange
      const page = mountPage({ $store });

      // Act & Assert
      expect(page.vm.mapFileTypeToDownloadType('docm')).toEqual('doc');
      expect(page.vm.mapFileTypeToDownloadType('jpeg')).toEqual('jpeg');
    });
  });

  describe('template', () => {
    it('will display the date subtext if there is a name', () => {
      // Arrange
      const data = () => ({
        name: 'Document1',
        comments: [],
        size: 1000000,
        dateString: 'translate_my_record.documents.documentPageSubtext 8 August 2019',
      });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');
      const dateString = 'translate_my_record.documents.documentPageSubtext 8 August 2019';

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual(dateString);
    });

    it('will display a comment when there is a single comment', () => {
      // Arrange
      const document = {
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
      };
      const documentComments = [{
        consultationHeaders: [{
          comments: [
            'test',
          ],
          header: 'Document',
          observationsWithTerm: [
            {
              codeId: 1234,
              eventGuid: 'test',
              term: 'test',
            }],
        },
        ],
      }];
      $store = newStore({ document, documentComments });
      const data = () => ({
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
        comments: ['this is a test'],
      });
      const page = mountPage({ $store, data });

      // Act
      const documentComment = page.find('#documentComment0 p');

      // Assert
      expect(documentComment.exists()).toBe(true);
      expect(documentComment.text()).toBe('this is a test');
    });

    it('will display three comments when there are three', () => {
      // Arrange
      const document = {
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
      };
      const documentComments = [{
        documentKey: {
          eventGuid: 'test',
          term: 'test',
          codeId: 1234,
        },
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
      }];
      $store = newStore({ document, documentComments });
      const data = () => ({
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
      });
      const page = mountPage({ $store, data });

      // Act
      const firstDocumentComment = page.find('#documentComment0 p');
      const secondDocumentComment = page.find('#documentComment1 p');
      const thirdDocumentComment = page.find('#documentComment2 p');

      // Assert
      expect(firstDocumentComment.exists()).toBe(true);
      expect(firstDocumentComment.text()).toBe('this is a test');

      expect(secondDocumentComment.exists()).toBe(true);
      expect(secondDocumentComment.text()).toBe('this is a second test');

      expect(thirdDocumentComment.exists()).toBe(true);
      expect(thirdDocumentComment.text()).toBe('this is a third test');
    });

    it('will not display the comments when there are none', () => {
      // Arrange
      const document = {
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
      };
      $store = newStore({ document });
      const data = () => ({
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
        comments: [],
      });
      const page = mountPage({ $store, data });

      // Act
      const documentComment = page.find('#documentComment0 p');

      // Assert
      expect(documentComment.exists()).toBe(false);
    });

    it('will not display the date subtext if there is no name', () => {
      // Arrange
      const data = () => ({
        name: undefined,
        comments: [],
        size: 1000000,
      });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(false);
    });

    it('will display the actions for the document', () => {
      // Arrange
      const page = mountPage({ $store });

      // Act
      const viewItem = page.find('#btn_viewDocument');
      const downloadItem = page.find('#btn_downloadDocument');

      // Assert
      expect(viewItem.text()).toEqual('translate_my_record.documents.actions.view');
      expect(downloadItem.text()).toEqual('translate_my_record.documents.actions.download');
      expect(viewItem.exists()).toBe(true);
      expect(downloadItem.exists()).toBe(true);
    });

    it('will not display the actions for the document if the document is too large', () => {
      const data = () => ({
        name: undefined,
        comments: [],
        size: 4000000,
      });

      // Arrange
      const page = mountPage({ $store, data });

      // Act
      const viewItem = page.find('#btn_viewDocument');
      const downloadItem = page.find('#btn_downloadDocument');

      // Assert
      expect(viewItem.exists()).toBe(false);
      expect(downloadItem.exists()).toBe(false);
    });

    it('will display a different header if the document is too large', async () => {
      // Arrange
      const document = { name: undefined, size: 4000000, date: { value: '2019-08-08T12:03:44+00:00' } };
      const documentComments = [{
        documentKey: {
          eventGuid: 'test',
          term: 'test',
          codeId: 1234,
        },
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
      }];
      $store = newStore({ document, documentComments });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'translate_my_record.documents.documentTooLargeHeader');
    });

    it('will display a different subtext if the document is too large', async () => {
      // Arrange
      const data = () => ({ size: 4000000, comments: [] });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual('translate_my_record.documents.documentTooLargeSubtext');
    });

    it('will not display the glossary or the warning if the document is too large', async () => {
      // Arrange
      const data = () => ({ size: 4000000, comments: [] });
      const page = mountPage({ $store, data });

      // Act
      const downloadWarning = page.find('#downloadWarning');
      const glossary = page.find('#glossary');

      // Assert
      expect(glossary.exists()).toBe(false);
      expect(downloadWarning.exists()).toBe(false);
    });
  });
});
