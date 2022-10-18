export const ADD_ERROR = 'ADD_ERROR';
export const ADD_ERROR_REPLY = 'ADD_ERROR_REPLY';
export const CLEAR_ERROR_REPLY = 'CLEAR_ERROR_REPLY';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const LOADED_MESSAGE = 'LOADED_MESSAGE';
export const SENDER_MESSAGES_LOADED = 'SENDER_MESSAGES_LOADED';
export const LOADED_SENDERS = 'LOADED_SENDERS';
export const SET_HAS_UNREAD = 'SET_HAS_UNREAD';
export const SET_UNREADMESSAGE_SENDER_COUNT = 'UNREAD_MESSAGE_SENDER_COUNT';
export const DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT = 'DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT';
export const SET_PREVIOUS_CHOICE = 'SET_PREVIOUS_CHOICE';

export const initialState = () => ({
  error: null,
  errorReply: null,
  errorReplyCount: 0,
  message: null,
  senderMessages: [],
  senderMessagesLoaded: false,
  senders: [],
  hasUnread: false,
  totalUnreadMessageCount: 0,
  totalUnreadSendersCount: 0,
  previousChoice: null,
});
