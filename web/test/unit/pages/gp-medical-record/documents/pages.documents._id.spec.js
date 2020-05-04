import each from 'jest-each';
import { mount, createRouter, createStore } from '../../../helpers';
import DocumentInformation from '@/pages/gp-medical-record/documents/_id';
import { DOCUMENT_DETAIL } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';

jest.mock('@/lib/sessionStorage');

let redirect;
const $router = createRouter();
const $route = {
  params: {
    id: 1,
  },
};

const newStore = ({
  document,
  documentConsultationsWithComments = [],
} = {}) => (
  createStore({
    $env: {
      CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
    },
    state: {
      myRecord: {
        hasAcceptedTerms: true,
        documentConsultationsWithComments,
      },
      documents: {
        currentDocument: document,
      },
      device: {
        isNativeApp: false,
      },
    },
  }));

const mountPage = ({ $store = newStore(), data = () => ({ comments: [] }) } = {}) =>
  mount(DocumentInformation, { $store, $router, $route, data });

describe('document view', () => {
  beforeEach(() => {
    redirect = jest.fn();
    hasAgreedToMedicalWarning.mockClear();
    hasAgreedToMedicalWarning.mockReturnValue(true);
  });

  describe('template', () => {
    it('will display Unknown Date if there is no document date', async () => {
      // Arrange
      const data = () => ({
        term: 'Document1',
        comments: [],
        size: 1000000,
        type: 'jpg',
        dateString: 'translate_my_record.documents.documentPageSubtext Unknown Date',
        isValidFile: true,
      });
      const page = mountPage({ data });

      // Act
      const documentInfo = page.find('#documentInfo p');
      const dateString = 'translate_my_record.documents.documentPageSubtext Unknown Date';

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual(dateString);
    });

    it('will display the date subtext if there is a term', () => {
      // Arrange
      const data = () => ({
        term: 'Document1',
        comments: [],
        size: 1000000,
        type: 'jpg',
        dateString: 'translate_my_record.documents.documentPageSubtext 8 August 2019',
        isValidFile: true,
      });
      const page = mountPage({ data });

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
      const page = mountPage({ $store: newStore({ document, documentComments }), data });

      // Act
      const documentComment = page.find('#documentComment0 pre');

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
      const page = mountPage({ $store: newStore({ document, documentComments }), data });

      // Act
      const firstDocumentComment = page.find('#documentComment0 pre');
      const secondDocumentComment = page.find('#documentComment1 pre');
      const thirdDocumentComment = page.find('#documentComment2 pre');

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
      const page = mountPage({ $store: newStore({ document }), data });

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
      const page = mountPage({ data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(false);
    });

    each([
      ['valid, viewable and downloadable', true, true, true],
      ['valid, not viewable but downloadable', true, false, true],
      ['valid, viewable but not downloadable', true, true, false],
      ['invalid, not viewable and not downloadable', false, false, false],
      ['invalid, viewable and downloadable', false, true, true],
    ]).it('will display the correct actions for the document when file is %s', (_, isValidFile, isViewable, isDownloadable) => {
      const data = () => ({
        name: undefined,
        comments: [],
        size: 1000000,
        type: 'jpg',
        isValidFile,
        isViewable,
        isDownloadable,
      });
      // Arrange
      const page = mountPage({ data });

      // Act
      const viewItem = page.find('#btn_viewDocument');
      const downloadItem = page.find('#btn_downloadDocument');

      // Assert
      if (isValidFile && isViewable) {
        expect(viewItem.text()).toEqual('translate_my_record.documents.actions.view');
      }

      if (isValidFile && isDownloadable) {
        expect(downloadItem.text()).toEqual('translate_my_record.documents.actions.download');
      }
      expect(viewItem.exists()).toBe(isValidFile && isViewable);
      expect(downloadItem.exists()).toBe(isValidFile && isDownloadable);
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
    }]).it('will display a different subtext if the document file type is TGA or TPIC', (testData) => {
      // Arrange
      const { size, comments, type } = testData;
      const data = () => ({ type, size, comments });
      const page = mountPage({ data });

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
    }]).it('will not display the glossary or the warning if the document file type is TGA or TPIC', (testData) => {
      // Arrange
      const { size, comments, type } = testData;
      const data = () => ({ size, comments, type });
      const page = mountPage({ data });

      // Act
      const downloadWarning = page.find('#downloadWarning');
      const glossary = page.find('#glossary');

      // Assert
      expect(glossary.exists()).toBe(false);
      expect(downloadWarning.exists()).toBe(false);
    });
  });

  describe('methods', () => {
    it('will navigate to the view document page with the correct id in the path', () => {
      // Arrange
      const route = { name: DOCUMENT_DETAIL.name, params: { id: 1 } };
      const page = mountPage();

      // Act
      page.vm.navigateToView();

      // Assert
      expect($router.push).toHaveBeenCalledWith(route);
    });

    it('will map the download type correctly', async () => {
      // Arrange
      const page = mountPage();

      // Act & Assert
      expect(page.vm.mapFileTypeToDownloadType('docm')).toEqual('doc');
      expect(page.vm.mapFileTypeToDownloadType('jpeg')).toEqual('jpeg');
    });

    it('will display a different header if the file is invalid', async () => {
      // Arrange
      const document = {
        name: undefined,
        type: 'jpg',
        date: { value: '2019-08-08T12:03:44+00:00' },
        documentType: 'Letter',
        isValidFile: false,
      };
      const documentComments = [{
        documentKey: {
          eventGuid: 'test',
          term: 'test',
          codeId: 1234,
        },
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
      }];
      const $store = newStore({ document, documentComments });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, route: $route, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'translate_my_record.documents.documentTypeUnavailableHeader');
      expect($store.dispatch).toHaveBeenCalledWith('pageTitle/updatePageTitle', 'translate_my_record.documents.documentTypeUnavailablePageTitle');
    });

    it('will set the header and page title to the document term', async () => {
      // Arrange
      const document = {
        term: 'Document1',
        type: 'jpg',
        size: 1000000,
        date: { value: '2019-08-08T12:03:44+00:00' },
        isValidFile: true,
        isViewable: true,
        isDownloadable: true,
      };
      const $store = newStore({ document });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, route: $route, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'Document1');
    });

    it('will dispatch the load document function if needMoreInformation is true', async () => {
      // Arrange
      const document = {
        name: 'Document1',
        type: 'jpg',
        needMoreInformation: true,
        date: { value: '2019-08-08T12:03:44+00:00' },
        isValidFile: true,
      };
      const $store = newStore({ document });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, route: $route, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('documents/loadDocument', { documentIdentifier: 1, updateMetaData: true });
    });

    it('will set the header to the document date if no name exists', async () => {
      // Arrange
      const document = {
        term: undefined,
        type: 'jpg',
        size: 1000000,
        date: { value: '2019-08-08T12:03:44+00:00' },
        isValidFile: true,
        isViewable: true,
        isDownloadable: true,
        documentType: null,
      };
      const $store = newStore({ document });
      const page = mountPage({ $store });

      const dateString = 'translate_my_record.documents.documentPageSubtext 8 August 2019';

      // Act
      await page.vm.$options.asyncData({ store: $store, route: $route, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', dateString);
    });

    it('will set the header to the letter date if documentType exists and is letter', async () => {
      // Arrange
      const document = {
        name: undefined,
        type: 'jpg',
        size: 1000000,
        date: { value: '2019-08-08T12:03:44+00:00' },
        isValidFile: true,
        isViewable: true,
        isDownloadable: true,
        documentType: 'Letter',
      };
      const $store = newStore({ document });
      const page = mountPage({ $store });

      const dateString = 'Letter translate_my_record.documents.docTypePageSubtext 8 August 2019';

      // Act
      await page.vm.$options.asyncData({ store: $store, route: $route, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', dateString);
    });
  });
});
