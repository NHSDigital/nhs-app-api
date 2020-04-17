import DownloadAttachment from '@/pages/patient-practice-messaging/download-attachment';
import { mount, createStore } from '../../helpers';

describe('patient practice messaging urgency page', () => {
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
        patientPracticeMessaging: {
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
    });
  };

  describe('back link clicked', () => {
    it('will redirect to patient practice messaging inbox', () => {
      mountPage();
      wrapper.vm.backToMessageClicked();
      expect(router.go).toHaveBeenCalled();
    });
  });

  describe('information paragraph', () => {
    it('will show information before download', () => {
      mountPage();
      const downloadInformation = wrapper.find('#downloadInformation');
      expect(downloadInformation.text()).toEqual('translate_patient_practice_messaging.downloadAttachment.downloadWarning');
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
