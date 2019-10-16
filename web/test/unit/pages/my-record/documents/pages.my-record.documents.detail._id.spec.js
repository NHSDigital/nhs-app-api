import each from 'jest-each';
import DocumentPage from '@/pages/my-record/documents/detail/_id';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../../helpers';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import NativeCallbacks from '@/services/native-app';

jest.mock('@/lib/sessionStorage');
jest.mock('@/services/native-app');

let page;
let $store;
const route = {
  params: { id: 'document-id' },
  query: { type: 'img', name: 'query file' },
};

const mountPage = () => {
  page = shallowMount(DocumentPage, {
    $store,
    $route: route,
  });
};

describe('my-record documents', () => {
  beforeEach(() => {
    $store = createStore({
      $env: { MY_RECORD_DOCUMENTS_ENABLED: true },
      state: {
        myRecord: initialState(),
        device: { isNativeApp: false },
      },
    });
    hasAgreedToMedicalWarning.mockClear();
    hasAgreedToMedicalWarning.mockReturnValue(true);
  });

  describe('asyncData', () => {
    const redirect = jest.fn();

    beforeEach(() => redirect.mockClear());

    describe('redirect', () => {
      it('will redirect to my-record if feature toggle is off', async () => {
        $store.app.$env.MY_RECORD_DOCUMENTS_ENABLED = false;
        await DocumentPage.asyncData({ redirect, store: $store });
        expect(redirect).toHaveBeenCalledWith('/my-record');
      });
      it('will redirect to my-record if not accepted terms and not hasAgreedToMedicalWarning', async () => {
        hasAgreedToMedicalWarning.mockReturnValue(false);
        await DocumentPage.asyncData({ redirect, store: $store });
        expect(redirect).toHaveBeenCalledWith('/my-record');
      });
      it('will load document and not redirect if toggle on and accepted terms even if type and name not set', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        await DocumentPage.asyncData({ redirect, route, store: $store });
        expect(redirect).not.toHaveBeenCalled();
        expect($store.dispatch).toHaveBeenCalledWith('myRecord/loadDocument', route.params.id);
      });
    });
  });
  describe('created', () => {
    beforeEach(() => {
      NativeCallbacks.hideHeader.mockClear();
      NativeCallbacks.hideMenuBar.mockClear();
    });
    it('will hide header and menubar if document exists and on client', () => {
      process.client = true;
      $store.state.myRecord.document.data = {};
      mountPage();
      expect(NativeCallbacks.hideHeader).toHaveBeenCalled();
      expect(NativeCallbacks.hideMenuBar).toHaveBeenCalled();
    });
    each([{
      client: false,
      document: {},
    }, {
      client: true,
      document: undefined,
    }]).it('will not hide header and menubar if document absent or not on client', ({ client, document }) => {
      process.client = client;
      $store.state.myRecord.document.data = document;
      mountPage();
      expect(NativeCallbacks.hideHeader).not.toHaveBeenCalled();
      expect(NativeCallbacks.hideMenuBar).not.toHaveBeenCalled();
    });
  });
  describe('data', () => {
    it('will set documents path to documents route with current id as #', () => {
      mountPage();
      expect(page.vm.documentsPath).toEqual('/my-record/documents#document-document-id');
    });
  });
  describe('beforeRouteLeave', () => {
    const next = jest.fn();
    beforeEach(() => {
      next.mockClear();
    });
    each([
      '/login', '/logout',
    ]).it('will not show header and menubar if going to LOGIN or LOGOUT routes', (to) => {
      DocumentPage.beforeRouteLeave(to, undefined, next);
      expect(NativeCallbacks.showHeader).not.toHaveBeenCalled();
      expect(NativeCallbacks.showMenuBar).not.toHaveBeenCalled();
      expect(next).toHaveBeenCalled();
    });
    it('will show header and menubar if not going to LOGIN or LOGOUT routes', () => {
      DocumentPage.beforeRouteLeave('/appointments', undefined, next);
      expect(NativeCallbacks.showHeader).not.toHaveBeenCalled();
      expect(NativeCallbacks.showMenuBar).not.toHaveBeenCalled();
      expect(next).toHaveBeenCalled();
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
    describe('document', () => {
      it('will render the document as html', () => {
        $store.state.myRecord.document.data = '<div id="document-content"><h1>This is a document</h1></div>';
        mountPage();
        const documentContent = page.find('div[id="document-content"]');
        expect(documentContent.exists()).toBe(true);
        expect(documentContent.find('h1').element.innerHTML).toEqual('This is a document');
      });
    });
  });
});
