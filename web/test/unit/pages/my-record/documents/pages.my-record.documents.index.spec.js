/* eslint-disable import/no-extraneous-dependencies */
// import Vue from 'vue';
import each from 'jest-each';
import chunk from 'lodash/fp/chunk';
import DocumentsPage from '@/pages/my-record/documents/index';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../../helpers';

jest.mock('lodash/fp/chunk');
let chunkCallback;

let page;
let $store;

const mountPage = () => {
  page = shallowMount(DocumentsPage, {
    $store,
  });
};

describe('my-record documents', () => {
  beforeEach(() => {
    $store = createStore({
      $env: { MY_RECORD_DOCUMENTS_ENABLED: true },
      state: { myRecord: initialState() },
    });

    chunkCallback = jest.fn().mockReturnValue([]);
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
    it('will load record if not loaded and my record terms are accepted', async () => {
      $store.state.myRecord.hasAcceptedTerms = true;

      await DocumentsPage.asyncData({ redirect, store: $store });

      expect($store.dispatch).toHaveBeenCalledWith('myRecord/load');
    });
    each([
      { acceptedTerms: false, loaded: false },
      { acceptedTerms: false, loaded: true },
      { acceptedTerms: true, loaded: true },
    ]).it('will not load record if loaded or terms are not accepted', async ({
      acceptedTerms,
      loaded,
    }) => {
      $store.state.myRecord.hasAcceptedTerms = acceptedTerms;
      $store.state.myRecord.hasLoaded = loaded;

      mountPage();

      expect($store.dispatch).not.toHaveBeenCalledWith('myRecord/load');
    });
    it('will redirect to my-record if no documents found in record', async () => {
      $store.state.myRecord.hasAcceptedTerms = true;

      await DocumentsPage.asyncData({ redirect, store: $store });

      expect(redirect).toHaveBeenCalledWith('/my-record');
    });
  });
  describe('data', () => {
    it('will chunk my-record documents into chunks of 2', () => {
      const data = ['data', 'to', 'be', 'chunked'];
      $store.state.myRecord.record.documents = { data };

      mountPage();

      expect(chunk).toHaveBeenCalledWith(2);
      expect(chunkCallback).toHaveBeenCalledWith(data);
      expect(page.vm.documentChunks).toEqual([]);
    });
  });
  describe('template', () => {
    describe('glossary', () => {
      it('will display an abbreviations glossary', () => {
        mountPage();

        const glossaryExists = page.find('glossary-stub').exists();
        expect(glossaryExists).toBe(true);
      });
    });
    describe('document items', () => {
      it('will render a card group for every chunk', () => {
        chunkCallback.mockReturnValue([[], []]);
        mountPage();

        const cardGroups = page.findAll('card-group-stub');
        expect(cardGroups.length).toEqual(2);
      });
      it('will render a document item in a card group item for each chunk item', () => {
        chunkCallback.mockReturnValue([
          [{ documentGuid: '1', extension: 'pdf', effectiveDate: {}, size: 10 }],
          [{ documentGuid: '3', extension: 'pdf', effectiveDate: {}, size: 10 }],
        ]);
        mountPage();

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
        chunkCallback.mockReturnValue([[document]]);
        mountPage();

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
