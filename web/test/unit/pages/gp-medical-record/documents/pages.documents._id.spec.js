import each from 'jest-each';
import { mount, createRouter, createStore } from '../../../helpers';
import DocumentInformation from '@/pages/gp-medical-record/documents/_id';
import { DOCUMENT_DETAIL, GP_MEDICAL_RECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';

jest.mock('@/lib/sessionStorage');

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
      MY_RECORD_DOCUMENTS_ENABLED_SUPPLIERS: ['EMIS'],
      CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
    },
    state: {
      myRecord: {
        document,
        hasAcceptedTerms: true,
        documentConsultationsWithComments,
        record: {
          supplier: 'EMIS',
        },
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
  });

  describe('asyncData', () => {
    it('will redirect to the my record page if the document environment variable is not enabled', async () => {
      // Arrange
      $store = newStore({ isDocumentsEnabled: false });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect(redirect).toBeCalledWith(GP_MEDICAL_RECORD.path);
    });

    it('will display Unknown Date if there is no document date', async () => {
      // Arrange
      const data = () => ({
        name: 'Document1',
        comments: [],
        size: 1000000,
        type: 'jpg',
        dateString: 'translate_my_record.documents.documentPageSubtext Unknown Date',
        isValidFile: true,
      });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');
      const dateString = 'translate_my_record.documents.documentPageSubtext Unknown Date';

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual(dateString);
    });

    it('will set the header and page title to the document name', async () => {
      // Arrange
      const document = { name: 'Document1',
        type: 'jpg',
        size: 1000000,
        date: { value: '2019-08-08T12:03:44+00:00' },
        isValidFile: true };
      $store = newStore({ document });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'Document1');
    });

    it('will set the header to the document date if no name exists', async () => {
      // Arrange
      const document = { name: undefined, type: 'jpg', size: 1000000, date: { value: '2019-08-08T12:03:44+00:00' }, isValidFile: true };
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
      const route = { name: DOCUMENT_DETAIL.name, params: { id: 1 } };
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
        type: 'jpg',
        dateString: 'translate_my_record.documents.documentPageSubtext 8 August 2019',
        isValidFile: true,
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
        type: 'jpg',
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
        type: 'jpg',
        comments: ['this is a test'],
        isValidFile: true,
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
        type: 'jpg',
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
        type: 'jpg',
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
        isValidFile: true,
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
        type: 'jpg',
        comments: [],
        isValidFile: true,
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
        type: 'jpg',
        isValidFile: true,
      });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(false);
    });

    it('will display the actions for the document', () => {
      const data = () => ({
        name: undefined,
        comments: [],
        size: 1000000,
        type: 'jpg',
        isValidFile: true,
      });
      // Arrange
      const page = mountPage({ $store, data });

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
        type: 'jpg',
        isValidFile: false,
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
      const document = { name: undefined, type: 'jpg', size: 4000000, date: { value: '2019-08-08T12:03:44+00:00' } };
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
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'translate_my_record.documents.documentUnavailableHeader');
      expect($store.dispatch).toHaveBeenCalledWith('pageTitle/updatePageTitle', 'translate_my_record.documents.documentUnavailablePageTitle');
    });

    each([{
      type: 'tga',
      size: 1000000,
      comments: [],
      isValidFile: true,
    }, {
      type: 'tpic',
      size: 1000000,
      comments: [],
      isValidFile: true,
    }, {
      type: 'jpg',
      size: 4000000,
      comments: [],
      isValidFile: true,
    }]).it('will display a different subtext if the document is too large or if the file type is TGA or TPIC', (testData) => {
      // Arrange
      const { size, comments, type } = testData;
      const data = () => ({ type, size, comments });
      const page = mountPage({ $store: newStore(), data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual('translate_my_record.documents.documentUnavailableSubtext');
    });

    each([{
      type: 'tga',
      size: 1000000,
      comments: [],
      isValidFile: true,
    }, {
      type: 'tpic',
      size: 1000000,
      comments: [],
      isValidFile: true,
    }, {
      type: 'jpg',
      size: 4000000,
      comments: [],
      isValidFile: true,
    }]).it('will not display the glossary or the warning if the document is too large or if the file type is TGA or TPIC', (testData) => {
      // Arrange
      const { size, comments, type } = testData;
      const data = () => ({ size, comments, type });
      const page = mountPage({ $store: newStore(), data });

      // Act
      const downloadWarning = page.find('#downloadWarning');
      const glossary = page.find('#glossary');

      // Assert
      expect(glossary.exists()).toBe(false);
      expect(downloadWarning.exists()).toBe(false);
    });
  });
});
