/* eslint-disable import/no-extraneous-dependencies */
// import Vue from 'vue';
import chunk from 'lodash/fp/chunk';
import DocumentsPage from '@/pages/gp-medical-record/documents/index';
import CardGroup from '@/components/widgets/card/CardGroup';
import { createStore, shallowMount } from '../../../helpers';
import Glossary from '@/components/Glossary';

jest.mock('lodash/fp/chunk');
let chunkCallback;

let page;
let $store;
const defaultDocuments = ['data', 'to', 'be', 'chunked'];
const documents = {
  data: defaultDocuments,
  hasErrored: false,
  hasAccess: true,
};

const expectedChunkedData = [['data', 'to'], ['be', 'chunked']];
const expectedPageData = { documentChunks: expectedChunkedData, documents };

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

    chunkCallback = jest.fn().mockReturnValue(expectedChunkedData);
    chunk.mockClear();
    chunk.mockReturnValue(chunkCallback);
  });

  describe('asyncData', () => {
    const redirect = jest.fn();

    beforeEach(() => redirect.mockClear());

    it('will redirect to gp-medical-record if feature toggle is off', async () => {
      $store.app.$env.MY_RECORD_DOCUMENTS_ENABLED = false;

      await DocumentsPage.asyncData({ redirect, store: $store });

      expect(redirect).toHaveBeenCalledWith('/gp-medical-record');
    });
    it('will chunk gp-medical-record documents into chunks of 2', async () => {
      const pageData = await DocumentsPage.asyncData({ store: $store });

      expect(chunk).toHaveBeenCalledWith(2);
      expect(chunkCallback).toHaveBeenCalledWith(defaultDocuments);
      expect(pageData).toEqual(expectedPageData);
    });
  });
  describe('template', () => {
    describe('glossary', () => {
      it('will display an abbreviations glossary', () => {
        mountPage({ data: () => ({ documents }) });

        const glossaryExists = page.find(Glossary).exists();
        expect(glossaryExists).toBe(true);
      });
    });
    describe('document items', () => {
      it('will render a card group for every chunk', () => {
        const documentChunks = [[], []];
        mountPage({ data: () => ({ documentChunks, documents }) });

        const cardGroups = page.findAll(CardGroup);
        expect(cardGroups.length).toEqual(2);
      });
      it('will render a document item in a card group item for each chunk item', () => {
        const documentChunks = [
          [{ documentGuid: '1', extension: 'pdf', effectiveDate: {}, size: 10 }],
          [{ documentGuid: '3', extension: 'pdf', effectiveDate: {}, size: 10 }],
        ];
        mountPage({ data: () => ({ documentChunks, documents }) });

        const cardGroupItems = page.findAll('card-group-stub card-group-item-stub');
        const firstDocumentItems = cardGroupItems.wrappers[0].find('document-item-stub[id="1"]');
        const secondDocumentItems = cardGroupItems.wrappers[1].find('document-item-stub[id="3"]');
        expect(cardGroupItems.length).toEqual(2);
        expect(firstDocumentItems.exists()).toBe(true);
        expect(secondDocumentItems.exists()).toBe(true);
      });
      it('will set appropriate attributes on a document item', () => {
        const document = {
          documentGuid: '1',
          extension: 'pdf',
          effectiveDate: { datePart: 'YearMonth' },
          size: 10,
          term: 'Document term',
          name: 'Document name',
          isAvailable: true,
        };
        mountPage({ data: () => ({ documentChunks: [[document]], documents }) });

        const documentItem = page.find('document-item-stub[id="1"]');
        expect(documentItem.vm.id).toEqual(document.documentGuid);
        expect(documentItem.vm.type).toEqual(document.extension);
        expect(documentItem.vm.date).toEqual(document.effectiveDate);
        expect(documentItem.vm.sizeInBytes).toEqual(document.size);
        expect(documentItem.vm.term).toEqual(document.term);
        expect(documentItem.vm.name).toEqual(document.name);
        expect(documentItem.vm.available).toEqual(document.isAvailable);
      });
    });
  });
});
