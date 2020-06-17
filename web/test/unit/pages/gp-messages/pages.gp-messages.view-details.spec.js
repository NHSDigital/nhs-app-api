import Message from '@/pages/messages/gp-messages/view-details';
import { redirectTo } from '@/lib/utils';
import { create$T, createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

describe('gp message details', () => {
  let wrapper;
  let store;
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
    loaded = true } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
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
        'serviceJourneyRules/deleteGpMessagesEnabled': deleteEnabled,
        'serviceJourneyRules/updateStatusGpMessagesEnabled': updateEnabled,
      },
    });
    $t = create$T();

    wrapper = mount(Message, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('created', () => {
    describe('selectedId is blank string', () => {
      beforeEach(async () => {
        mountPage({ selectedId: '' });
        await wrapper.vm.$nextTick();
      });

      it('will not dispatch gpMessages/loadMessage', () => {
        expect(store.dispatch).not.toHaveBeenCalled();
      });

      it('will redirect to /gp-messages', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
      });
    });

    describe.each([
      ['a default id for newly sent messages', '0'],
      ['a non-default id from the GPSS', '12345678-1234-abcd-ef12-345678901234'],
    ])('selectedId is %s (%s)', (_, id) => {
      beforeEach(async () => {
        mountPage({ selectedId: id });
        await wrapper.vm.$nextTick();
      });

      it('will clear back link override', () => {
        expect(store.dispatch).toHaveBeenCalledWith('navigation/clearBackLinkOverride');
      });
    });

    describe('selected message id is not 0 and messags details are undefined', () => {
      it('will dispatch `gpMessages/loadMessage` with id', () => {
        mountPage({ messages: undefined, selectedId: '1' });
        expect(store.dispatch).toBeCalledWith('gpMessages/loadMessage', { id: '1', clearApiError: true });
      });
    });

    describe('selected message id is not 0 and message details are undefined', () => {
      beforeEach(async () => {
        mountPage({ messages: undefined, selectedId: '1' });
        await wrapper.vm.$nextTick();
      });

      it('will load the message details', () => {
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/loadMessage', { id: '1', clearApiError: true });
      });

      it('will update the read status', () => {
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/updateReadStatusAsRead');
      });
    });

    describe.each([
      ['selected message id is 0 and messages are undefined', '0', undefined],
      ['selected message id is not 0 and messages are defined', 1, messageDetailsNoReplies],
    ])('%s', (_, selectedMessageId, messages) => {
      beforeEach(async () => {
        mountPage({ selectedId: selectedMessageId, messages });
        await wrapper.vm.$nextTick();
      });

      it('will not load the message', () => {
        expect(store.dispatch).not.toHaveBeenCalledWith('gpMessages/loadMessage', expect.anything());
      });

      it('will not mark the message as read', () => {
        expect(store.dispatch).not.toHaveBeenCalledWith('gpMessages/updateReadStatusAsRead');
      });
    });

    describe('selected message id is not 0 and message details are undefined and update read status is not enabled', () => {
      beforeEach(async () => {
        mountPage({ updateEnabled: false, messages: messageDetailsNoReplies, selectedId: '1' });
        await wrapper.vm.$nextTick();
      });

      it('will not update read status', () => {
        expect(store.dispatch).not.toHaveBeenCalledWith('gpMessages/updateReadStatusAsRead');
      });
    });
  });

  describe('template', () => {
    it('will show the delete button if the delete functionality is enabled', () => {
      mountPage({ messages: messageDetailsNoReplies, selectedId: '1' });
      expect(wrapper.find('#deleteMessage').exists()).toBe(true);
    });

    it('will hide the delete button if the delete functionality is disabled', () => {
      mountPage({ messages: messageDetailsNoReplies, deleteEnabled: false, selectedId: '1' });
      expect(wrapper.find('#deleteMessage').exists()).toBe(false);
    });

    it('will show the page divider if there are unread messages', () => {
      mountPage({ messages: messageDetailsUnreadReplies, selectedId: '1' });
      expect(wrapper.find('#receivedMessagesDivider').exists()).toBe(true);
    });

    it('will not show the page divider if there are only read messages', () => {
      mountPage({ messages: messageDetailsReadReplies, selectedId: '1' });
      expect(wrapper.find('#receivedMessagesDivider').exists()).toBe(false);
    });

    it('will show two read replies', () => {
      mountPage({ messages: messageDetailsReadReplies, selectedId: '1' });

      expect(wrapper.find('#initialSentMessage0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel2').exists()).toBe(false);
    });

    it('will show one read one unread replies', () => {
      mountPage({ messages: messageDetailsMixedReadReplies, selectedId: '1' });

      expect(wrapper.find('#initialSentMessage0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(false);
      expect(wrapper.find('#unreadMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#unreadMessageReplyPanel1').exists()).toBe(false);
    });

    it('will show initial as received if from supplier', () => {
      mountPage({ messages: messageDetailsInitialFromSupplier, selectedId: '1' });

      expect(wrapper.find('#initialMessageReplyPanel0').exists()).toBe(true);
    });

    it('will show correct message panels if one reply is from the patient', () => {
      mountPage({ messages: messageDetailsInitialFromSupplierWithPatientReply, selectedId: '1' });

      expect(wrapper.find('#initialMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel0').exists()).toBe(true);
      expect(wrapper.find('#readMessageReplyPanel1').exists()).toBe(false);
      expect(wrapper.find('#unreadReplySentMessage0').exists()).toBe(true);
    });

    it('will show the new message menu item', () => {
      mountPage({ messages: messageDetailsReadReplies, selectedId: '1', loaded: true });
      expect(wrapper.find('#newMessage').exists()).toBe(true);
    });

    it('will go to the urgency page when the new message menu item is clicked', () => {
      mountPage({ messages: messageDetailsReadReplies, selectedId: '1', loaded: true });
      wrapper.vm.sendNewMessageClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/urgency');
    });
  });
});
