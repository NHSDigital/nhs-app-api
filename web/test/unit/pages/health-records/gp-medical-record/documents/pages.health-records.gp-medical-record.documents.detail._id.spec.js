import * as dependency from '@/lib/utils';
import each from 'jest-each';
import DocumentPage from '@/pages/health-records/gp-medical-record/documents/detail/_id';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import NativeCallbacks from '@/services/native-app';
import Glossary from '@/components/Glossary';
import { createStore, shallowMount } from '../../../../helpers';

jest.mock('@/lib/sessionStorage');
jest.mock('@/services/native-app');

let page;
let $store;
let dummyMetaTag;
const route = {
  params: { id: 'document-id' },
  query: { type: 'img', name: 'query file' },
};

const mountPage = async () => {
  page = shallowMount(DocumentPage, {
    $store,
    $route: route,
  });
};

describe('health-records documents', () => {
  dummyMetaTag = document.createElement('meta');
  document.getElementsByName = jest.fn().mockReturnValue([dummyMetaTag]);
  beforeEach(() => {
    $store = createStore({
      state: {
        documents: {
          currentDocument: {},
        },
        device: { isNativeApp: false },
        myRecord: initialState(),
      },
    });
    hasAgreedToMedicalWarning.mockClear();
    hasAgreedToMedicalWarning.mockReturnValue(true);
  });

  describe('mounted', () => {
    dependency.redirectTo = jest.fn();

    beforeEach(() => {
      dependency.redirectTo.mockClear();
      NativeCallbacks.hideHeader.mockClear();
      NativeCallbacks.hideMenuBar.mockClear();
    });

    describe('redirect', () => {
      it('will redirect to health-records > gp-record if not accepted terms and not hasAgreedToMedicalWarning', async () => {
        hasAgreedToMedicalWarning.mockReturnValue(false);
        await mountPage();
        expect(dependency.redirectTo).toHaveBeenCalledWith(page.vm, 'health-records/gp-medical-record');
      });
      it('will load document and not redirect if toggle on and accepted terms even if type and name not set', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        await mountPage();
        expect(dependency.redirectTo).not.toHaveBeenCalled();
        expect($store.dispatch).toHaveBeenCalledWith('documents/loadDocument', { documentIdentifier: route.params.id });
      });
      it('will not load document if it is already loaded', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        $store.state.documents.currentDocument.data = 'testData';
        await mountPage();
        expect(dependency.redirectTo).not.toHaveBeenCalled();
        expect($store.dispatch).not.toHaveBeenCalled();
      });
      it('will hide header and menubar if document exists', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        $store.state.documents.currentDocument.data = {};
        await mountPage();
        expect(NativeCallbacks.hideHeader).toHaveBeenCalled();
        expect(NativeCallbacks.hideMenuBar).toHaveBeenCalled();
      });
      it('will set the meta tag content so the user can zoom in', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        $store.state.documents.currentDocument.data = {};
        await mountPage();
        expect(document.getElementsByName('viewport')[0].getAttribute('content'))
          .toEqual('width=device-width, initial-scale=1, minimum-scale=1.0, maximum-scale=10.0, user-scalable=yes');
      });
      it('will not hide header and menubar if document absent', async () => {
        $store.state.myRecord.hasAcceptedTerms = true;
        $store.state.documents.currentDocument.data = undefined;
        await mountPage();
        expect(NativeCallbacks.hideHeader).not.toHaveBeenCalled();
        expect(NativeCallbacks.hideMenuBar).not.toHaveBeenCalled();
      });
    });
  });
  describe('computed', () => {
    it('will return the document path', () => {
      mountPage();
      expect(page.vm.documentPath).toEqual('health-records/gp-medical-record/documents/document-id');
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
        $store.state.documents.currentDocument.data = '<div id="document-content"><h1>This is a document</h1></div>';
        mountPage();
        const documentContent = page.find('div[id="document-content"]');
        expect(documentContent.exists()).toBe(true);
        expect(documentContent.find('h1').element.innerHTML).toEqual('This is a document');
      });
    });
  });
});
