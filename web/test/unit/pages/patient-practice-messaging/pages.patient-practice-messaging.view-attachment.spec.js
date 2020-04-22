import ViewAttachment from '@/pages/patient-practice-messaging/view-attachment';
import { mount, createStore } from '../../helpers';

jest.mock('@/lib/utils');
jest.mock('@/services/native-app');

let dummyMetaTag;


describe('patient practice view attachment page', () => {
  let wrapper;
  dummyMetaTag = document.createElement('meta');
  document.getElementsByName = jest.fn().mockReturnValue([dummyMetaTag]);

  const router = {
    go: jest.fn(),
  };

  const mountPage = (store) => {
    wrapper = mount(ViewAttachment, {
      $store: store,
      $router: router,
    });
  };

  const selectedMessageDetails = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: true,
      replies: [],
    },
  };

  describe('back link clicked', () => {
    it('will redirect to patient practice messaging inbox', () => {
      const store = createStore({
        $env: {
          CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
        },
        state: {
          device: {
            isNativeApp: false,
          },
          patientPracticeMessaging: {
            selectedMessageDetails,
          },
          documents: {
            currentDocument: null,
          },
        },
      });
      mountPage(store);
      wrapper.vm.backToMessageClicked();
      expect(router.go).toHaveBeenCalled();
    });
  });

  describe('asyncData', () => {
    it('will redirect back to the inbox if there is no document', async () => {
      const redirect = jest.fn();
      const store = createStore({
        $env: {
          CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
        },
        state: {
          device: {
            isNativeApp: false,
          },
          patientPracticeMessaging: {
            selectedMessageDetails,
          },
          documents: {},
        },
      });
      mountPage(store);
      await wrapper.vm.$options.asyncData({ store, redirect });

      expect(redirect).toHaveBeenCalledWith('/patient-practice-messaging');
    });

    it('will set the invalid headers if the document is not viewable', async () => {
      const redirect = jest.fn();
      const store = createStore({
        $env: {
          CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
        },
        state: {
          device: {
            isNativeApp: false,
          },
          patientPracticeMessaging: {
            selectedMessageDetails,
          },
          documents: {
            currentDocument: {
              type: null,
            },
          },
        },
      });
      mountPage(store);
      await wrapper.vm.$options.asyncData({ store, redirect });

      expect(store.dispatch).toHaveBeenCalledWith('header/updateHeaderText',
        'translate_pageHeaders.patientPracticeMessagingAttachmentUnavailable');
      expect(store.dispatch).toHaveBeenCalledWith('pageTitle/updatePageTitle',
        'translate_pageTitles.patientPracticeMessagingAttachmentUnavailable');
    });
  });
});
