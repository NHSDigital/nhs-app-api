export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const SET_SENDER = 'SET_SENDER';
export const SET_HAS_UNREAD = 'SET_HAS_UNREAD';

export const initialState = () => ({
  senderMessages: [],
  selectedSender: '',
  hasUnread: false,
});
