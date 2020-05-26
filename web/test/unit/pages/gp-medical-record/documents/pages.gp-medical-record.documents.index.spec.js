/* eslint-disable import/no-extraneous-dependencies */
import DocumentsPage from '@/pages/health-records/gp-medical-record/documents/index';
import { createStore, shallowMount } from '../../../helpers';

let page;
let $store;
const defaultDocuments = [{
  extension: 'jpg',
  effectiveDate: { value: '2019-08-08T12:03:44+00:00' },
  type: 'Letter',
  name: 'namey boi',
  codeId: 7362,
  term: 'termy boi',
  eventGuid: 'fd2e843d-2a99-427a-a58f-f7d6d45c8f39',
  size: 20000,
  isValidFile: true,
  comments: 'commenty boi',
  needMoreInformation: false,
  documentIdentifier: 'yippppe',
}];
const documents = {
  data: defaultDocuments,
  hasErrored: false,
  hasAccess: true,
};

const mountPage = ({ data } = {}) => {
  page = shallowMount(DocumentsPage, {
    $store,
    data,
  });
};

describe('gp-medical-record documents', () => {
  beforeEach(() => {
    $store = createStore({
      $env: {
        CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
      },
      state: {
        device: { isNativeApp: false },
        myRecord: {
          record: {
            documents,
          },
        },
      },
    });
  });

  describe('template', () => {
    describe('document items', () => {
      it('will render a menu item for every document', () => {
        const theDocuments = { data: [
          { documentIdentifier: '1', extension: 'pdf', effectiveDate: { value: '2019-08-08T12:03:44+00:00' }, size: 10 },
          { documentIdentifier: '3', extension: 'pdf', effectiveDate: { value: '2019-08-08T12:03:44+00:00' }, size: 10 },
        ] };

        mountPage({ data: () => ({ documents: theDocuments }) });

        const menuItems = page.findAll('menu-item-list-stub menu-item-stub');
        const firstDocumentItems = menuItems.wrappers[0].find('menu-item-stub[id="1"]');
        const secondDocumentItems = menuItems.wrappers[1].find('menu-item-stub[id="3"]');
        expect(menuItems.length).toEqual(2);
        expect(firstDocumentItems.exists()).toBe(true);
        expect(secondDocumentItems.exists()).toBe(true);
      });

      it('will set appropriate attributes on a document item that has a term', () => {
        const document = {
          documentIdentifier: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: 10,
          term: 'Document term',
        };
        const theDocuments = { data: [document] };
        mountPage({ data: () => ({ documents: theDocuments }) });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const titleString = 'Document term translate_my_record.documents.documentMenuItemTitle';
        const description = '(PDF, 10B)';

        expect(documentItem.vm.id).toEqual(document.documentIdentifier);
        expect(documentItem.vm.text).toEqual(titleString);
        expect(documentItem.vm.description).toEqual(description);
      });

      it('will set appropriate attributes on a document item that has no term', () => {
        const document = {
          documentIdentifier: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: 10,
        };

        const theDocuments = { data: [document] };
        mountPage({ data: () => ({ documents: theDocuments }) });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const dateString = '8 August 2019';
        const description = '(PDF, 10B)';
        expect(documentItem.vm.id).toEqual(document.documentIdentifier);
        expect(documentItem.vm.text).toEqual(dateString);
        expect(documentItem.vm.description).toEqual(description);
      });

      it('will set not show the size on a document item that has a null size', () => {
        const document = {
          documentIdentifier: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: null,
        };

        const theDocuments = { data: [document] };
        mountPage({ data: () => ({ documents: theDocuments }) });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const dateString = '8 August 2019';
        const description = '(PDF)';
        expect(documentItem.vm.id).toEqual(document.documentIdentifier);
        expect(documentItem.vm.text).toEqual(dateString);
        expect(documentItem.vm.description).toEqual(description);
      });
    });
  });

  describe('methods', () => {
    it('will set isViewable and isDownloadable to true', async () => {
      // Arrange
      mountPage({ $store, data: () => ({ documents }) });

      // Act
      await page.vm.documentClicked(defaultDocuments[0]);

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('documents/setSelectedDocumentInfo', {
        type: 'jpg',
        date: { value: '2019-08-08T12:03:44+00:00' },
        documentType: 'Letter',
        name: 'namey boi',
        codeId: 7362,
        term: 'termy boi',
        eventGuid: 'fd2e843d-2a99-427a-a58f-f7d6d45c8f39',
        size: 20000,
        isValidFile: true,
        comments: 'commenty boi',
        needMoreInformation: false,
        isDownloadable: true,
        isViewable: true,
      });
    });
  });
});
