/* eslint-disable import/no-extraneous-dependencies */
// import Vue from 'vue';
import DocumentsPage from '@/pages/gp-medical-record/documents/index';
import { createStore, shallowMount } from '../../../helpers';


let page;
let $store;
const defaultDocuments = ['data', 'to', 'be', 'chunked'];
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
        MY_RECORD_DOCUMENTS_ENABLED: true,
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
    describe('asyncData', () => {
      const redirect = jest.fn();
      beforeEach(() => redirect.mockClear());

      it('will redirect to my-record if feature toggle is off', async () => {
        $store.app.$env.MY_RECORD_DOCUMENTS_ENABLED = false;

        await DocumentsPage.asyncData({ redirect, store: $store });

        expect(redirect).toHaveBeenCalledWith('/gp-medical-record');
      });
    });
    describe('document items', () => {
      it('will render a menu item for every document', () => {
        const theDocuments = { data: [
          { documentGuid: '1', extension: 'pdf', effectiveDate: { value: '2019-08-08T12:03:44+00:00' }, size: 10 },
          { documentGuid: '3', extension: 'pdf', effectiveDate: { value: '2019-08-08T12:03:44+00:00' }, size: 10 },
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
          documentGuid: '1',
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

        expect(documentItem.vm.id).toEqual(document.documentGuid);
        expect(documentItem.vm.text).toEqual(titleString);
        expect(documentItem.vm.description).toEqual(description);
      });
      it('will set appropriate attributes on a document item that has no term', () => {
        const document = {
          documentGuid: '1',
          extension: 'pdf',
          effectiveDate: { value: '2019-08-08T12:03:44+00:00', datePart: 'YearMonth' },
          size: 10,
        };

        const theDocuments = { data: [document] };
        mountPage({ data: () => ({ documents: theDocuments }) });

        const documentItem = page.find('menu-item-stub[id="1"]');
        const dateString = '8 August 2019';
        const description = '(PDF, 10B)';
        expect(documentItem.vm.id).toEqual(document.documentGuid);
        expect(documentItem.vm.text).toEqual(dateString);
        expect(documentItem.vm.description).toEqual(description);
      });
    });
  });
});
