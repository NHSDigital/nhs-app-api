/* eslint-disable no-param-reassign */
import each from 'jest-each';
import buildMessageMetadata from '@/lib/gp-messages/build-message-metadata';

describe('build-message-metadata', () => {
  let state;

  const setState = (stateModifier) => {
    state = {
      unreadIndex: 0,
      readIndex: 0,
      selectedMessageDetails: {
        messageDetails: {
          messageId: 'some-id',
          content: 'initial message jobby',
          outboundMessage: true,
          isUnread: false,
          replies: [
            {
              replyContent: 'one contenty boy',
              outboundMessage: false,
              isUnread: false,
            },
          ],
        },
      },
    };

    if (typeof stateModifier === 'function') {
      stateModifier(state);
    }
  };

  beforeEach(() => setState());

  each([
    ['key', 'initialMessage'],
    ['index', 0],
    ['prefixIdentifier', 'initial'],
    ['content', 'initial message jobby'],
    ['isFirstUnreadMessage', false],
  ]).it('creates messages for single read inbound message with correct %s', (field, value) => {
    setState((s) => { s.selectedMessageDetails.messageDetails.replies = []; });

    buildMessageMetadata(state);

    expect(state.messages[0][field]).toEqual(value);
  });

  each([
    ['key', 'initialMessage'],
    ['index', 0],
    ['prefixIdentifier', 'initial'],
    ['content', 'initial message jobby'],
    ['isFirstUnreadMessage', true],
  ]).it('creates messages for single unread inbound message with correct %s', (field, value) => {
    setState((s) => {
      s.selectedMessageDetails.messageDetails.isUnread = true;
      s.selectedMessageDetails.messageDetails.replies = [];
    });

    buildMessageMetadata(state);

    expect(state.messages[0][field]).toEqual(value);
  });

  each([
    ['key', [
      'initialMessage',
      'readMessage0',
      'readMessageReply1',
      'unreadMessageReply0',
      'unreadMessageReply1',
      'unreadMessage2',
    ]],
    ['index', [
      0,
      0,
      1,
      0,
      1,
      2,
    ]],
    ['prefixIdentifier', [
      'initial',
      'read',
      'readReply',
      'unreadReply',
      'unreadReply',
      'unread',
    ]],
    ['content', [
      'initial message jobby',
      'one contenty boy',
      'read reply eijit',
      'some reply eijit',
      'another reply eijit',
      'last reply eijit',
    ]],
    ['isFirstUnreadMessage', [
      false,
      false,
      false,
      true,
      false,
      false,
    ]],
  ]).it('creates messages for inbound message and replies with correct %s field values', (field, values) => {
    setState((s) => {
      s.selectedMessageDetails.messageDetails.replies.push({
        replyContent: 'read reply eijit',
        outboundMessage: true,
        isUnread: false,
      });

      s.selectedMessageDetails.messageDetails.replies.push({
        replyContent: 'some reply eijit',
        outboundMessage: true,
        isUnread: true,
      });

      s.selectedMessageDetails.messageDetails.replies.push({
        replyContent: 'another reply eijit',
        outboundMessage: true,
        isUnread: true,
      });

      s.selectedMessageDetails.messageDetails.replies.push({
        replyContent: 'last reply eijit',
        outboundMessage: false,
        isUnread: true,
      });
    });

    buildMessageMetadata(state);

    values.forEach((v, idx) => expect(state.messages[idx][field]).toEqual(v));
  });
});
