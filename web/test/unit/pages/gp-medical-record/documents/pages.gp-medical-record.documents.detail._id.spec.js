import each from 'jest-each';
import DocumentPage from '@/pages/gp-medical-record/documents/detail/_id';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../../helpers';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import NativeCallbacks from '@/services/native-app';
import Glossary from '@/components/Glossary';

jest.mock('@/lib/sessionStorage');
jest.mock('@/services/native-app');

let page;
let $store;
let dummyMetaTag;
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

describe('gp-medical-record documents', () => {
  dummyMetaTag = document.createElement('meta');
  document.getElementsByName = jest.fn().mockReturnValue([dummyMetaTag]);
  beforeEach(() => {
    $store = createStore({
      $env: {
        CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
      },
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
      it('will redirect to gp-medical-record if not accepted terms and not hasAgreedToMedicalWarning', async () => {
        hasAgreedToMedicalWarning.mockReturnValue(false);
        await DocumentPage.asyncData({ redirect, store: $store });
        expect(redirect).toHaveBeenCalledWith('/gp-medical-record');
      });
      it('will load document and not redirect if toggle on and accepted terms even if type and name not set', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        await DocumentPage.asyncData({ redirect, route, store: $store });
        expect(redirect).not.toHaveBeenCalled();
        expect($store.dispatch).toHaveBeenCalledWith('myRecord/loadDocument', route.params.id);
      });

      it('will not load document if it is already loaded', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        $store.state.myRecord.document.data = 'testData';
        await DocumentPage.asyncData({ redirect, route, store: $store });
        expect(redirect).not.toHaveBeenCalled();
        expect($store.dispatch).not.toHaveBeenCalled();
      });
    });
  });
  describe('computed', () => {
    it('will return the document path', () => {
      mountPage();
      expect(page.vm.documentPath).toEqual('/gp-medical-record/documents/document-id');
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
    it('will set the meta tag content so the user can zoom in', () => {
      process.client = true;
      $store.state.myRecord.document.data = {};
      mountPage();
      expect(document.getElementsByName('viewport')[0].getAttribute('content'))
        .toEqual('width=device-width, initial-scale=1, minimum-scale=1.0, maximum-scale=10.0, user-scalable=yes');
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
  describe('methods', () => {
    it('will set the viewport accordingly when setZoom is called', () => {
      mountPage();
      page.vm.setZoom(true);
      expect(document.getElementsByName('viewport')[0].getAttribute('content'))
        .toEqual('width=device-width, initial-scale=1, minimum-scale=1.0, maximum-scale=10.0, user-scalable=yes');

      page.vm.setZoom(false);
      expect(document.getElementsByName('viewport')[0].getAttribute('content'))
        .toEqual('width=device-width, initial-scale=1, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0');
    });
  });
  describe('beforeDestroy', () => {
    it('will set the content on the viewport so that the user cannot zoom in', () => {
      mountPage();
      page.destroy();
      expect(document.getElementsByName('viewport')[0].getAttribute('content'))
        .toEqual('width=device-width, initial-scale=1, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0');
    });
  });
  describe('beforeRouteLeave', () => {
    const next = jest.fn();
    beforeEach(() => {
      next.mockClear();
    });
    each(['/login', '/logout'])
      .it('will not show header and menubar if going to LOGIN or LOGOUT routes', (to) => {
        DocumentPage.beforeRouteLeave({ path: to }, undefined, next);
        expect(NativeCallbacks.showHeader).not.toHaveBeenCalled();
        expect(NativeCallbacks.showMenuBar).not.toHaveBeenCalled();
        expect(next).toHaveBeenCalled();
      });
    it('will show header and menubar if not going to LOGIN or LOGOUT routes', () => {
      DocumentPage.beforeRouteLeave.call(
        { navHidden: true }, { path: '/appointments' }, undefined, next,
      );
      expect(NativeCallbacks.showHeader).toHaveBeenCalled();
      expect(NativeCallbacks.showMenuBar).toHaveBeenCalled();
      expect(next).toHaveBeenCalled();
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
