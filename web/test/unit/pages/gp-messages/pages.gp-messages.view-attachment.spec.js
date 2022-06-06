import ViewAttachment from '@/pages/messages/gp-messages/view-attachment';
import { redirectTo } from '@/lib/utils';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount, createStore } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

jest.mock('@/lib/utils');
jest.mock('@/services/native-app');

const messageDetails = {
  messageDetails: {
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    replies: [],
  },
};

describe('gp messages view attachment page', () => {
  let wrapper;
  let store;

  document.getElementsByName = jest.fn().mockReturnValue([document.createElement('meta')]);

  const router = {
    go: jest.fn(),
  };

  const mountPage = ({
    selectedMessageDetails,
    currentDocument,
  } = {}) => {
    store = createStore({
      state: {
        device: { isNativeApp: false },
        gpMessages: { selectedMessageDetails },
        documents: { currentDocument },
      },
      $env: {
        CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/',
      },
    });

    wrapper = mount(ViewAttachment, {
      $store: store,
      $router: router,
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('back link clicked', () => {
    it('will redirect to gp messages inbox', () => {
      mountPage({ currentDocument: {} });
      const backDataPurpose = '[data-purpose=main-back-button]';
      const backLink = wrapper.find(backDataPurpose);
      expect(backLink.attributes('href')).toBe('messages/gp-messages/view-details');
    });
  });

  describe('created', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
    });

    it('will redirect to gp-messages if there is no document', () => {
      mountPage();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
    });

    describe('document is not viewable', () => {
      beforeEach(() => {
        mountPage({
          currentDocument: { isViewable: false },
          selectedMessageDetails: messageDetails,
        });
      });

      it('will emit UPDATE_HEADER with attachment unavailable as event when not viewable', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'navigation.pages.headers.gpMessagesAttachmentCannotView');
      });

      it('will emit UPDATE_TITLE with attachment unavailable as event when not viewable', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'navigation.pages.titles.gpMessagesAttachmentCannotView');
      });
    });

    describe('document is viewable', () => {
      beforeEach(() => {
        mountPage({
          currentDocument: { isViewable: true },
          selectedMessageDetails: messageDetails,
        });
      });

      it('will not emit an UPDATE_HEADER or UPDATE_TITLE event', () => {
        expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_HEADER, expect.anything());
        expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_TITLE, expect.anything());
      });
    });
  });
});
