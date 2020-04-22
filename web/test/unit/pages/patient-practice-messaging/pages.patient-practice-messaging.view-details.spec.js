import Message from '@/pages/patient-practice-messaging/view-details';
import { create$T, createStore, mount } from '../../helpers';

describe('patient messaging messages', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const messageDetailsNoReplies = [{
    messageId: '1',
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    replies: [],
    isUnread: false,
    key: 'initialMessage',
    index: 0,
    prefixIdentifier: 'initial',
    isFirstUnreadMessage: false,
  }];

  const messageDetailsUnreadReplies = [{
    messageId: '1',
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    isUnread: false,
    key: 'initialMessage',
    index: 0,
    prefixIdentifier: 'initial',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: true,
    key: 'unreadMessageReply0',
    index: 0,
    prefixIdentifier: 'unreadReply',
    isFirstUnreadMessage: true,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: true,
    key: 'unreadMessageReply1',
    index: 1,
    prefixIdentifier: 'unreadReply',
    isFirstUnreadMessage: false,
  }];

  const messageDetailsReadReplies = [{
    messageId: '1',
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    isUnread: false,
    key: 'initialMessage',
    index: 0,
    prefixIdentifier: 'initial',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'readMessage0',
    index: 0,
    prefixIdentifier: 'read',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'readMessage1',
    index: 1,
    prefixIdentifier: 'read',
    isFirstUnreadMessage: false,
  }];

  const messageDetailsMixedReadReplies = [{
    messageId: '1',
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    isUnread: false,
    key: 'initialMessage',
    index: 0,
    prefixIdentifier: 'initial',
    isFirstUnreadMessage: false,
  },
  {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'readMessage0',
    index: 0,
    prefixIdentifier: 'read',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: true,
    key: 'unreadMessageReply0',
    index: 0,
    prefixIdentifier: 'unread',
    isFirstUnreadMessage: true,
  }];

  const messageDetailsInitialFromSupplier = [{
    messageId: '1',
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'initialMessage',
    index: 0,
    prefixIdentifier: 'initial',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'readMessage0',
    index: 0,
    prefixIdentifier: 'read',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: true,
    key: 'unreadMessageReply0',
    index: 0,
    prefixIdentifier: 'unreadReply',
    isFirstUnreadMessage: true,
  }];

  const messageDetailsInitialFromSupplierWithPatientReply = [{
    messageId: '1',
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'initialMessage',
    index: 0,
    prefixIdentifier: 'initial',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: false,
    isUnread: false,
    key: 'readMessage0',
    index: 0,
    prefixIdentifier: 'read',
    isFirstUnreadMessage: false,
  }, {
    sender: 'Test',
    content: 'This is a test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    isUnread: true,
    key: 'unreadMessageReply0',
    index: 0,
    prefixIdentifier: 'unreadReply',
    isFirstUnreadMessage: true,
  }];

  const mountPage = ({
    messages,
    deleteEnabled = true,
    updateEnabled = true,
    selectedId = undefined,
    loaded = false } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageDetails: Array.isArray(messages) && messages.length > 0
            ? messages[0]
            : undefined,
          selectedMessageId: selectedId,
          loadedDetails: loaded,
          selectedMessageRecipient: 'test',
          unreadIndex: 0,
          readIndex: 0,
          messages,
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
        mountPage({ messages: undefined, selectedId: '1' });
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
      mountPage({ messages: messageDetailsNoReplies, selectedId: '1', loaded: true });
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/updateReadStatusAsRead');
    });
  });

  describe('template', () => {
    it('will show the delete button if the delete functionality is enabled', () => {
      mountPage({ messages: messageDetailsNoReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#deleteMessage').exists()).toBe(true);
    });

    it('will hide the delete button if the delete functionality is disabled', () => {
      mountPage({ messages: messageDetailsNoReplies, deleteEnabled: false, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#deleteMessage').exists()).toBe(false);
    });

    it('will show the page divider if there are unread messages', () => {
      mountPage({ messages: messageDetailsUnreadReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#receivedMessagesDivider').exists()).toBe(true);
    });

    it('will not show the page divider if there are only read messages', () => {
      mountPage({ messages: messageDetailsReadReplies, toggle: true, selectedId: '1', loaded: true });
      expect(wrapper.find('#receivedMessagesDivider').exists()).toBe(false);
    });

    it('will show two read replies', () => {
      mountPage({ messages: messageDetailsReadReplies, toggle: true, selectedId: '1', loaded: true });

      expect(wrapper.find('#initialSentMessage0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel2').exists()).toBe(false);
    });

    it('will show one read one unread replies', () => {
      mountPage({ messages: messageDetailsMixedReadReplies, toggle: true, selectedId: '1', loaded: true });

      expect(wrapper.find('#initialSentMessage0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(false);
      expect(wrapper.find('#unreadMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#unreadMessageReplyPanel1').exists()).toBe(false);
    });

    it('will show initial as received if from supplier', () => {
      mountPage({ messages: messageDetailsInitialFromSupplier, toggle: true, selectedId: '1', loaded: true });

      expect(wrapper.find('#initialMessageReplyPanel0').exists()).toBe(true);
    });

    it('will show correct message panels if one reply is from the patient', () => {
      mountPage({ messages: messageDetailsInitialFromSupplierWithPatientReply, toggle: true, selectedId: '1', loaded: true });

      expect(wrapper.find('#initialMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(false);
      expect(wrapper.find('#unreadReplySentMessage0').exists()).toBe(true);
    });
  });
});
