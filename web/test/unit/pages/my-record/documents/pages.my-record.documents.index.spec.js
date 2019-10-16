/* eslint-disable import/no-extraneous-dependencies */
// import Vue from 'vue';
import chunk from 'lodash/fp/chunk';
import DocumentsPage from '@/pages/my-record/documents/index';
import CardGroup from '@/components/widgets/card/CardGroup';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../../helpers';
import Glossary from '@/components/Glossary';

jest.mock('lodash/fp/chunk');
let chunkCallback;

let page;
let $store;
const defaultDocuments = ['data', 'to', 'be', 'chunked'];
const expectedChunkedData = [['data', 'to'], ['be', 'chunked']];
const expectedPageData = { documentChunks: expectedChunkedData };

const mountPage = ({ data } = {}) => {
  page = shallowMount(DocumentsPage, {
    $store,
    data,
  });
};

describe('my-record documents', () => {
  beforeEach(() => {
    $store = createStore({
      $env: { MY_RECORD_DOCUMENTS_ENABLED: true },
      state: { myRecord: initialState(), device: { isNativeApp: false } },
    });

    chunkCallback = jest.fn().mockReturnValue(expectedChunkedData);
    chunk.mockClear();
    chunk.mockReturnValue(chunkCallback);
  });

  describe('asyncData', () => {
    const redirect = jest.fn();

    beforeEach(() => redirect.mockClear());

    it('will redirect to my-record if feature toggle is off', async () => {
      $store.app.$env.MY_RECORD_DOCUMENTS_ENABLED = false;

      await DocumentsPage.asyncData({ redirect, store: $store });

      expect(redirect).toHaveBeenCalledWith('/my-record');
    });
    it('will redirect to my-record if no documents found in record', async () => {
      $store.state.myRecord.hasAcceptedTerms = true;

      await DocumentsPage.asyncData({ redirect, store: $store });

      expect(redirect).toHaveBeenCalledWith('/my-record');
    });
    it('will chunk my-record documents into chunks of 2', async () => {
      $store.state.myRecord.record.documents = { data: defaultDocuments };

      const pageData = await DocumentsPage.asyncData({ store: $store });

      expect(chunk).toHaveBeenCalledWith(2);
      expect(chunkCallback).toHaveBeenCalledWith(defaultDocuments);
      expect(pageData).toEqual(expectedPageData);
    });
  });
  describe('template', () => {
    describe('glossary', () => {
      it('will display an abbreviations glossary', () => {
        mountPage();

        const glossaryExists = page.find(Glossary).exists();
        expect(glossaryExists).toBe(true);
      });
    });
    describe('document items', () => {
      it('will render a card group for every chunk', () => {
        const documentChunks = [[], []];
        mountPage({ data: () => ({ documentChunks }) });

        const cardGroups = page.findAll(CardGroup);
        expect(cardGroups.length).toEqual(2);
      });
      it('will render a document item in a card group item for each chunk item', () => {
        const documentChunks = [
          [{ documentGuid: '1', extension: 'pdf', effectiveDate: {}, size: 10 }],
          [{ documentGuid: '3', extension: 'pdf', effectiveDate: {}, size: 10 }],
        ];
        mountPage({ data: () => ({ documentChunks }) });

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
        mountPage({ data: () => ({ documentChunks: [[document]] }) });

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
