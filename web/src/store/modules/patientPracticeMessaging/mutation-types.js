export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const LOADED_MESSAGE = 'LOADED_MESSAGE';
export const CLEAR = 'CLEAR';
export const SET_SELECTED_MESSAGE_ID = 'SET_SELECTED_MESSAGE_ID';
export const SET_DETAILS = 'SET_DETAILS';
export const SET_SUMMARIES = 'SET_SUMMARIES';
export const SET_SELECTED_MESSAGE_RECIPIENT = 'SET_SELECTED_MESSAGE_RECIPIENT';

export const initialState = () => ({
  loaded: false,
  messageSummaries: [],
  selectedMessageDetails: undefined,
  selectedMessageId: undefined,
  selectedMessageRecipient: undefined,
  loadedDetails: false,
});
