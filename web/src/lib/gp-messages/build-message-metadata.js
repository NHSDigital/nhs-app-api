/* eslint-disable no-param-reassign */
import { isBlankString } from '@/lib/utils';

function setContent(message) {
  if (isBlankString(message.content)
    && !isBlankString(message.replyContent)) {
    message.content = message.replyContent;
  }
}

function buildPrefixIdentifier(message, initialMessageId) {
  if (!isBlankString(message.messageId)
    && message.messageId === initialMessageId) {
    message.prefixIdentifier = 'initial';
    return;
  }

  message.prefixIdentifier = `${message.unreadPrefix}read${message.replyPostFix}`;
}

function isFirstUnreadMessage(message) {
  message.isFirstUnreadMessage = message.isUnread && message.index === 0;
}

function buildKey(state, message, initialMessageId) {
  if (!isBlankString(message.messageId)
    && message.messageId === initialMessageId) {
    message.index = 0;
    message.unreadPrefix = '';
    message.replyPostFix = '';

    message.key = 'initialMessage';
    return;
  }

  if (message.isUnread) {
    message.index = state.unreadIndex;
    message.unreadPrefix = 'un';

    state.unreadIndex += 1;
  } else {
    message.index = state.readIndex;
    message.unreadPrefix = '';

    state.readIndex += 1;
  }

  message.replyPostFix = message.outboundMessage ? 'Reply' : '';
  message.key = `${message.unreadPrefix}readMessage${message.replyPostFix}${message.index}`;
}

export default function buildMessageMetadata(state) {
  const initialMessage = state.selectedMessageDetails.messageDetails;
  const messages = [initialMessage].concat(initialMessage.replies || []);

  state.readIndex = 0;
  state.unreadIndex = 0;

  messages.forEach((m) => {
    buildKey(state, m, initialMessage.messageId);
    isFirstUnreadMessage(m);
    buildPrefixIdentifier(m, initialMessage.messageId);
    setContent(m);
  });

  state.messages = messages;
}
