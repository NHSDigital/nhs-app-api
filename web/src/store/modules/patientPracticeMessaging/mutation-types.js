export const INIT = 'INIT';
export const CLEAR = 'CLEAR';
export const LOADED_MESSAGES = 'LOADED_MESSAGES';
export const LOADED_RECIPIENTS = 'LOADED_RECIPIENTS';
export const LOADED_MESSAGE = 'LOADED_MESSAGE';
export const SET_SUMMARIES = 'SET_SUMMARIES';
export const SET_RECIPIENTS = 'SET_RECIPIENTS';
export const SET_DETAILS = 'SET_DETAILS';
export const SET_SELECTED_MESSAGE_ID = 'SET_SELECTED_MESSAGE_ID';
export const SET_SELECTED_MESSAGE_RECIPIENT = 'SET_SELECTED_MESSAGE_RECIPIENT';
export const SET_URGENCY_CHOICE = 'SET_URGENCY_CHOICE';
export const SET_STATUS_STATE = 'SET_STATUS_STATE';
export const MESSAGE_SENT = 'MESSAGE_SENT';

export const initialState = () => ({
  loadedMessages: false,
  loadedDetails: false,
  loadedRecipients: false,
  messageSummaries: [],
  messageRecipients: [],
  selectedMessageDetails: undefined,
  selectedMessageId: undefined,
  selectedMessageRecipient: undefined,
  urgencyChoice: undefined,
  statusState: undefined,
  messageSent: false,
});
