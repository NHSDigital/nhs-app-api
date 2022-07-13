export const ADD_ERROR = 'ADD_ERROR';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const LOADED_MESSAGE = 'LOADED_MESSAGE';
export const SENDER_MESSAGES_LOADED = 'SENDER_MESSAGES_LOADED';
export const LOADED_SENDERS = 'LOADED_SENDERS';
export const SET_HAS_UNREAD = 'SET_HAS_UNREAD';

export const initialState = () => ({
  error: null,
  message: null,
  senderMessages: [],
  senderMessagesLoaded: false,
  senders: [],
  hasUnread: false,
});
