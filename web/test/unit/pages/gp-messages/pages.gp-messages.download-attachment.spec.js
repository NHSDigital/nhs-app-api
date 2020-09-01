import DownloadAttachment from '@/pages/messages/gp-messages/download-attachment';
import i18n from '@/plugins/i18n';
import { redirectTo } from '@/lib/utils';
import { mount, createStore } from '../../helpers';

jest.mock('@/lib/utils', () => {
  const { datePart, mimeType } = jest.requireActual('@/lib/utils');

  return {
    redirectTo: jest.fn(),
    datePart,
    mimeType,
  };
});

describe('gp messages urgency page', () => {
  let wrapper;
  let store;
  const router = {
    go: jest.fn(),
  };
  const route = {
    params: { date: '2019-12-09T13:56:50.377Z' },
  };

  const mountPage = ({ isNativeApp = false } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
          attachmentId: '1',
        },
        device: {
          isNativeApp,
        },
        documents: {
          currentDocument: {
            fileName: 'Test attachment',
            mimeType: '',
            type: 'jpg',
            isNativeApp,
          },
        },
      },
    });
    wrapper = mount(DownloadAttachment, {
      $store: store,
      $router: router,
      $route: route,
      mountOpts: { i18n },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('back link clicked', () => {
    it('will redirect to gp messages inbox', () => {
      mountPage();
      wrapper.vm.backToMessageClicked();
      expect(router.go).toHaveBeenCalled();
    });
  });

  describe('information paragraph', () => {
    it('will show information before download', () => {
      mountPage();
      const downloadInformation = wrapper.find('#downloadInformation');
      expect(downloadInformation.text()).toEqual('When you download this file, you become responsible for keeping it confidential.');
      expect(downloadInformation.exists()).toBe(true);
    });
  });

  describe('download button', () => {
    it('will show a download button', async () => {
      mountPage();
      const downloadButton = wrapper.find('#downloadButton');
      expect(downloadButton.exists()).toBe(true);
    });

    it('will dispatch the download action when clickes', async () => {
      mountPage();
      const downloadButton = wrapper.find('#downloadButton');
      downloadButton.trigger('click');

      expect(store.dispatch).toHaveBeenCalledWith('documents/downloadDocument',
        { documentIdentifier: '1',
          fileName: 'Attachment_9 December 2019',
          fileExtension: 'jpg',
          mimeType: 'image/jpeg',
          isNative: false });
    });
  });
});
