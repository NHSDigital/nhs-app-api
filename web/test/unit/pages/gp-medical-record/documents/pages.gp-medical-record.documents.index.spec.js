/* eslint-disable import/no-extraneous-dependencies */
import DocumentsPage from '@/pages/health-records/gp-medical-record/documents/index';
import { createStore, shallowMount } from '../../../helpers';

let page;
let $store;

const createDocument = ({
  documentIdentifier = 'yippppe',
  extension = 'jpg',
  effectiveDate = { value: '2019-08-08T12:03:44+00:00' },
  size = 20000,
  term = 'termy boi',
  type = 'Letter',
} = {}) =>
  ({
    extension,
    effectiveDate,
    type,
    name: 'namey boi',
    codeId: 7362,
    term,
    eventGuid: 'fd2e843d-2a99-427a-a58f-f7d6d45c8f39',
    size,
    isValidFile: true,
    comments: 'commenty boi',
    needMoreInformation: false,
    documentIdentifier,
  });

const mountPage = ({ documentData = [createDocument()] } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          supplier: 'EMIS',
          documents: {
            data: documentData,
            hasErrored: false,
            hasAccess: true,
          },
        },
      },
    },
  });

  page = shallowMount(DocumentsPage, {
    $store,
  });
};

describe('gp-medical-record documents', () => {
  describe('template', () => {
    describe('document items', () => {
      it('will render a menu item for every document', async () => {
        mountPage({ documentData: [
          createDocument({ documentIdentifier: '1', extension: 'pdf', effectiveDate: { value: '2019-08-08T12:03:44+00:00' }, size: 10 }),
          createDocument({ documentIdentifier: '3', extension: 'pdf', effectiveDate: { value: '2019-08-08T12:03:44+00:00' }, size: 10 }),
        ],
        });
        await page.vm.$nextTick();

        const menuItems = page.findAll('menu-item-list-stub menu-item-stub');
        const firstDocument = menuItems.wrappers[0].find('menu-item-stub[id="1"]');
        const secondDocument = menuItems.wrappers[1].find('menu-item-stub[id="3"]');
        expect(menuItems.length).toEqual(2);
        expect(firstDocument.exists()).toBe(true);
        expect(secondDocument.exists()).toBe(true);
      });

      it('will set appropriate attributes on a document item that has a term', async () => {
        const document = createDocument({
          documentIdentifier: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: 10,
          term: 'Document term',
        });

        mountPage({ documentData: [document] });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const titleString = 'Document term translate_my_record.documents.documentMenuItemTitle';
        const description = '(PDF, 10B)';
        expect(documentItem.vm.id).toEqual(document.documentIdentifier);
        expect(documentItem.vm.text).toEqual(titleString);
        expect(documentItem.vm.description).toEqual(description);
      });

      it('will set appropriate attributes on a document item that has no term', async () => {
        const document = createDocument({
          documentIdentifier: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: 10,
          term: null,
          type: null,
        });

        mountPage({ documentData: [document] });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const dateString = '8 August 2019';
        const description = '(PDF, 10B)';
        expect(documentItem.vm.id).toEqual(document.documentIdentifier);
        expect(documentItem.vm.text).toEqual(dateString);
        expect(documentItem.vm.description).toEqual(description);
      });

      it('will set not show the size on a document item that has a null size', async () => {
        const document = createDocument({
          documentIdentifier: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: null,
          term: null,
          type: null,
        });

        mountPage({ documentData: [document] });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const dateString = '8 August 2019';
        const description = '(PDF)';
        expect(documentItem.vm.id).toEqual('1');
        expect(documentItem.vm.text).toEqual(dateString);
        expect(documentItem.vm.description).toEqual(description);
      });
    });
  });

  describe('methods', () => {
    it('will set isViewable and isDownloadable to true', async () => {
      // Arrange
      const defaultDocuments = [createDocument()];
      mountPage({ documentData: defaultDocuments });

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
