import Message from '@/pages/patient-practice-messaging/view-details';
import { create$T, createStore, mount } from '../../helpers';

describe('patient messaging messages', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const messageDetailsNoReplies = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: true,
      messageReplies: [],
    },
  };

  const messageDetailsUnreadReplies = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: true,
      messageReplies: [{
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: true,
      }, {
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: true,
      }],
    },
  };

  const messageDetailsReadReplies = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: true,
      messageReplies: [{
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: false,
      }, {
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: false,
      }],
    },
  };

  const messageDetailsMixedReadReplies = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: true,
      messageReplies: [{
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: false,
      }, {
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: true,
      }],
    },
  };

  const messageDetailsInitialFromSupplier = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: false,
      messageReplies: [{
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: false,
      }, {
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: true,
      }],
    },
  };

  const messageDetailsInitialFromSupplierWithPatientReply = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
      outboundMessage: false,
      messageReplies: [{
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: false,
        isUnread: false,
      }, {
        sender: 'Test',
        replyContent: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377',
        outboundMessage: true,
        isUnread: true,
      }],
    },
  };


  const mountPage = ({
    messageDetails,
    deleteEnabled = true,
    updateEnabled = true,
    selectedId = undefined,
    loaded = false } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageDetails: messageDetails,
          selectedMessageId: selectedId,
          loadedDetails: loaded,
          selectedMessageRecipient: 'test',
        },
        device: { isNativeApp: false },
      },
      getters: {
        'serviceJourneyRules/deletePatientPracticeMessageEnabled': deleteEnabled,
        'serviceJourneyRules/updateStatusPatientPracticeMessageEnabled': updateEnabled,
      },
    });
    $t = create$T();

    wrapper = mount(Message, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('fetch', () => {
    describe('selected message id is defined', () => {
      beforeEach(async () => {
        mountPage({ messageDetails: undefined, selectedId: '1' });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will dispatch `patientPracticeMessaging/loadMessage` with id', () => {
        expect(store.dispatch).toBeCalledWith('patientPracticeMessaging/loadMessage', { id: '1', clearApiError: true });
      });
    });

    describe('selected message id is undefined', () => {
      beforeEach(async () => {
        mountPage();
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will not dispatch load', () => {
        expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/loadMessage');
      });

      it('will redirect to /patient-practice-messaging', () => {
        expect(redirect).toHaveBeenCalledWith('/patient-practice-messaging');
      });
    });
  });

  describe('mounted', () => {
    it('will dispatch update read status', () => {
      mountPage({ messageDetails: messageDetailsNoReplies, selectedId: '1', loaded: true });
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/updateReadStatusAsRead');
    });
  });

  describe('template', () => {
    it('will show the delete button if the delete functionality is enabled', () => {
      mountPage({ messageDetails: messageDetailsNoReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#deleteMessage').exists()).toBe(true);
    });

    it('will hide the delete button if the delete functionality is disabled', () => {
      mountPage({ messageDetails: messageDetailsNoReplies, deleteEnabled: false, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#deleteMessage').exists()).toBe(false);
    });

    it('will show the page divider if there are unread messages', () => {
      mountPage({ messageDetails: messageDetailsUnreadReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#receivedMessagesDivider').exists()).toBe(true);
    });

    it('will not show the page divider if there are only read messages', () => {
      mountPage({ messageDetails: messageDetailsReadReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#receivedMessagesDivider').exists()).toBe(false);
    });

    it('will show two read replies', () => {
      mountPage({ messageDetails: messageDetailsReadReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#initialSentMessage0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel2').exists()).toBe(false);
    });

    it('will show one read one unread replies', () => {
      mountPage({ messageDetails: messageDetailsMixedReadReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#initialSentMessage0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(false);
      expect(wrapper.find('#unreadMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#unreadMessageReplyPanel1').exists()).toBe(false);
    });

    it('will show initial as received if from supplier', () => {
      mountPage({ messageDetails: messageDetailsInitialFromSupplier, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#initialMessageReplyPanel0').exists()).toBe(true);
    });

    it('will show correct message panels if one reply is from the patient', () => {
      mountPage({ messageDetails: messageDetailsInitialFromSupplierWithPatientReply, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#initialMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(false);
      expect(wrapper.find('#unreadReplySentMessage0').exists()).toBe(true);
    });
  });
});
