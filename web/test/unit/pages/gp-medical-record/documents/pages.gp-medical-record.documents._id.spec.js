import each from 'jest-each';
import DocumentInformation from '@/pages/health-records/gp-medical-record/documents/_id';
import { DOCUMENT_DETAIL_NAME } from '@/router/names';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import * as dependancy from '@/services/event-bus';
import * as utilsDependancy from '@/lib/utils';
import { mount, createRouter, createStore, create$T } from '../../../helpers';

jest.mock('@/lib/sessionStorage');

const $router = createRouter();
const $route = {
  params: {
    id: 1,
  },
};
const $t = create$T();

const newStore = ({
  document,
  documentConsultationsWithComments = [],
} = {}) => (
  createStore({
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

let page;

const mountPage = async ({ $store = newStore() } = {}) => {
  page = mount(DocumentInformation, { $store, $router, $route, $t });
  await page.vm.$nextTick();
};

describe('document view', () => {
  beforeEach(() => {
    dependancy.EventBus = { $emit: jest.fn() };
    utilsDependancy.createRouteByNameObject = jest.fn(r => r);
    hasAgreedToMedicalWarning.mockClear();
    hasAgreedToMedicalWarning.mockReturnValue(true);
  });

  describe('template', () => {
    it('will display Unknown Date if there is no document date', async () => {
      // Arrange
      const document = {
        term: 'Document1',
        comments: [],
        size: 1000000,
        type: 'jpg',
        date: {},
        isValidFile: true,
      };
      await mountPage({ $store: newStore({ document }) });

      // Act
      const documentInfo = page.find('#documentInfo p');
      const dateString = 'translate_my_record.documents.documentPageSubtext Unknown Date';

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual(dateString);
    });

    it('will display the date subtext if there is a term', async () => {
      // Arrange
      const document = {
        term: 'Document1',
        comments: [],
        size: 1000000,
        type: 'jpg',
        date: { value: '2019-08-08T12:03:44+00:00' },
        isValidFile: true,
      };

      // Act
      await mountPage({ $store: newStore({ document }) });

      const documentInfo = page.find('#documentInfo p');
      const dateString = 'translate_my_record.documents.documentPageSubtext 8 August 2019';

      // Assert
      expect(documentInfo.exists()).toBe(true);
      expect(documentInfo.text()).toEqual(dateString);
    });

    it('will display a comment when there is a single comment', async () => {
      // Arrange
      const document = {
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
        type: 'jpg',
        comments: ['this is a test'],
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

      // Act
      await mountPage({ $store: newStore({ document, documentComments }) });

      const documentComment = page.find('#documentComment0 pre');

      // Assert
      expect(documentComment.exists()).toBe(true);
      expect(documentComment.text()).toBe('this is a test');
    });

    it('will display three comments when there are three', async () => {
      // Arrange
      const document = {
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
        type: 'jpg',
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
      };
      const documentComments = [{
        documentKey: {
          eventGuid: 'test',
          term: 'test',
          codeId: 1234,
        },
        comments: ['this is a test', 'this is a second test', 'this is a third test'],
      }];

      await mountPage({ $store: newStore({ document, documentComments }) });

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

    it('will not display the comments when there are none', async () => {
      // Arrange
      const document = {
        name: 'Doc1',
        date: { value: '2019-08-08T12:03:44+00:00' },
        term: 'test',
        eventGuid: 'test',
        codeId: 1234,
        comments: [],
      };

      await mountPage({ $store: newStore({ document }) });

      // Act
      const documentComment = page.find('#documentComment0 p');

      // Assert
      expect(documentComment.exists()).toBe(false);
    });

    it('will not display the date subtext if there is no name', async () => {
      // Arrange
      const document = {
        name: undefined,
        comments: [],
        size: 1000000,
        type: 'jpg',
        isValidFile: true,
      };
      await mountPage({ $store: newStore({ document }) });

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
    ]).it('will display the correct actions for the document when file is %s', async (_, isValidFile, isViewable, isDownloadable) => {
      const document = {
        name: undefined,
        comments: [],
        size: 1000000,
        type: 'jpg',
        isValidFile,
        isViewable,
        isDownloadable,
      };
      // Arrange
      await mountPage({ $store: newStore({ document }) });

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
    }]).it('will display a different subtext if the document file type is TGA or TPIC', async (testData) => {
      // Arrange
      const { size, comments, type } = testData;
      const document = { type, size, comments };
      await mountPage({ $store: newStore({ document }) });

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
    }]).it('will not display the glossary or the warning if the document file type is TGA or TPIC', async (testData) => {
      // Arrange
      const { size, comments, type } = testData;
      const document = { size, comments, type };
      await mountPage({ $store: newStore({ document }) });

      // Act
      const downloadWarning = page.find('#downloadWarning');
      const glossary = page.find('#glossary');

      // Assert
      expect(glossary.exists()).toBe(false);
      expect(downloadWarning.exists()).toBe(false);
    });

    it('will not display any content when loading', async () => {
      await mountPage();
      page.vm.loading = true;

      expect(page.find('[data-purpose=page-content]').exists()).toBe(false);
    });
  });

  describe('methods', () => {
    it('will navigate to the view document page with the correct id in the path', async () => {
      // Arrange
      const store = newStore();
      const route = { name: DOCUMENT_DETAIL_NAME, params: { id: 1 }, store };
      await mountPage({ $store: store });

      // Act
      page.vm.navigateToView();

      // Assert
      expect(utilsDependancy.createRouteByNameObject).toHaveBeenCalledWith(route);
    });

    it.each([
      ['jpeg', 'jpeg'],
      ['docm', 'doc'],
      ['jfif', 'jpg'],
    ])('will map the download type correctly', async (type, expectedType) => {
      // Arrange
      await mountPage();

      // Act & Assert
      expect(page.vm.mapFileTypeToDownloadType(type)).toEqual(expectedType);
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

      // Act
      await mountPage({ $store });

      // Assert
      expect(dependancy.EventBus.$emit).toHaveBeenCalledWith(dependancy.UPDATE_HEADER, 'translate_my_record.documents.documentTypeUnavailableHeader', true);
      expect(dependancy.EventBus.$emit).toHaveBeenCalledWith(dependancy.UPDATE_TITLE, 'translate_my_record.documents.documentTypeUnavailablePageTitle', true);
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

      // Act
      await mountPage({ $store });

      // Assert
      expect(dependancy.EventBus.$emit).toHaveBeenCalledWith(dependancy.UPDATE_HEADER, 'Document1', true);
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

      // Act
      await mountPage({ $store });

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

      // Act
      await mountPage({ $store });

      const dateString = 'translate_my_record.documents.documentPageSubtext 8 August 2019';

      // Assert
      expect(dependancy.EventBus.$emit)
        .toHaveBeenCalledWith(dependancy.UPDATE_HEADER, dateString, true);
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

      // Act
      await mountPage({ $store });

      const dateString = 'Letter translate_my_record.documents.docTypePageSubtext 8 August 2019';

      // Assert
      expect(dependancy.EventBus.$emit)
        .toHaveBeenCalledWith(dependancy.UPDATE_HEADER, dateString, true);
    });
  });
});
