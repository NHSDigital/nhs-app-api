import { mount, createRouter, createStore } from '../../../helpers';
import DocumentInformation from '@/pages/my-record/documents/_id';
import { MY_RECORD_DOCUMENT_DETAIL, MYRECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { datePart } from '@/lib/utils';

jest.mock('@/lib/sessionStorage');
jest.mock('@/lib/utils');

let redirect;
const $router = createRouter();
const $route = {
  params: {
    id: 1,
  },
};

const newStore = ({ isDocumentsEnabled = true, document } = {}) => (
  createStore({
    $env: {
      MY_RECORD_DOCUMENTS_ENABLED: isDocumentsEnabled,
    },
    state: {
      myRecord: {
        document,
        hasAcceptedTerms: true,
      },
      device: {
        isNativeApp: false,
      },
    },
  }));

const mountPage = ({ $store, data = undefined }) =>
  mount(DocumentInformation, { $store, $router, $route, data });

describe('document view', () => {
  let $store;

  beforeEach(() => {
    redirect = jest.fn();
    hasAgreedToMedicalWarning.mockClear();
    hasAgreedToMedicalWarning.mockReturnValue(true);
    datePart.mockReturnValue('8 August 2019');
  });

  describe('asyncData', () => {
    it('will redirect to the my record page if the document environment variable is not enabled', async () => {
      // Arrange
      $store = newStore(false);
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect(redirect).toBeCalledWith(MYRECORD.path);
    });

    it('will redirect to the my record page if there is no document date', async () => {
      // Arrange
      const document = { date: { value: undefined } };
      $store = newStore(document);
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect(redirect).toBeCalledWith(MYRECORD.path);
    });

    it('will set the header and page title to the document name', async () => {
      // Arrange
      const document = { name: 'Document1', date: { value: '2019-08-08T12:03:44+00:00' } };
      $store = newStore({ document });
      const page = mountPage({ $store });

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'Document1');
    });

    it('will set the header to the document date if no name exists', async () => {
      // Arrange
      const document = { name: undefined, date: { value: '2019-08-08T12:03:44+00:00' } };
      $store = newStore({ document });
      const page = mountPage({ $store });

      const dateString = 'translate_my_record.documents.documentPageSubtext 8 August 2019';

      // Act
      await page.vm.$options.asyncData({ store: $store, redirect });

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', dateString);
    });
  });

  describe('methods', () => {
    it('will navigate to the view document page with the correct id in the path', () => {
      // Arrange
      const route = { name: MY_RECORD_DOCUMENT_DETAIL.name, params: { id: 1 } };
      const page = mountPage({ $store });

      // Act
      page.vm.navigateToView();

      // Assert
      expect($router.push).toHaveBeenCalledWith(route);
    });
    it('will convert map the download type correctly', async () => {
      // Arrange
      const page = mountPage({ $store });

      // Act & Assert
      expect(page.vm.mapFileTypeToDownloadType('docm')).toEqual('doc');
      expect(page.vm.mapFileTypeToDownloadType('jpeg')).toEqual('jpeg');
    });
  });

  describe('template', () => {
    it('will display the date subtext if there is a name', () => {
      // Arrange
      const data = () => ({
        name: 'Document1',
      });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(true);
    });

    it('will not display the date subtext if there is no name', () => {
      // Arrange
      const data = () => ({
        name: undefined,
      });
      const page = mountPage({ $store, data });

      // Act
      const documentInfo = page.find('#documentInfo p');

      // Assert
      expect(documentInfo.exists()).toBe(false);
    });

    it('will display a download warning', () => {
      // Arrange
      const data = () => ({
        name: undefined,
      });
      const page = mountPage({ $store, data });

      // Act
      const downloadWarning = page.find('#downloadWarning');

      // Assert
      expect(downloadWarning.exists()).toBe(true);
      expect(downloadWarning.text()).toEqual('translate_my_record.documents.downloadWarning');
    });

    it('will display the actions for the document', () => {
      // Arrange
      const page = mountPage({ $store });

      // Act
      const viewItem = page.find('#btn_viewDocument');
      const downloadItem = page.find('#btn_downloadDocument');

      // Assert
      expect(viewItem.text()).toEqual('translate_my_record.documents.actions.view');
      expect(downloadItem.text()).toEqual('translate_my_record.documents.actions.download');
      expect(viewItem.exists()).toBe(true);
      expect(downloadItem.exists()).toBe(true);
    });
  });
});
