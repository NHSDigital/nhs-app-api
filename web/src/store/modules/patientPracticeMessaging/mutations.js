import mapKeys from 'lodash/fp/mapKeys';
import {
  INIT,
  CLEAR,
  LOADED_MESSAGES,
  LOADED_RECIPIENTS,
  LOADED_MESSAGE,
  SET_SUMMARIES,
  SET_RECIPIENTS,
  SET_DETAILS,
  SET_SELECTED_MESSAGE_ID,
  SET_SELECTED_MESSAGE_RECIPIENT,
  SET_URGENCY_CHOICE,
  MESSAGE_SENT,
  SET_DELETED,
  CLEAR_SELECTED_MESSAGE_DETAILS,
  CLEAR_SELECTED_RECIPIENT,
  SET_ATTACHMENT_ID,
  initialState,
} from './mutation-types';
import buildMessageMetadata from '@/lib/patient-practice-messaging/build-message-metadata';

const clearMessage = (state) => {
  state.selectedMessageDetails = undefined;
  state.selectedMessageId = undefined;
  state.selectedMessageRecipient = undefined;
  state.messageSent = false;
};

const clearSelectedMessageDetails = (state) => {
  state.selectedMessageDetails = undefined;
};

export default {
  [INIT](state) {
    const blank = initialState();
    return mapKeys((key) => {
      state[key] = blank[key];
    })(state);
  },
  [CLEAR](state) {
    clearMessage(state);
  },
  [CLEAR_SELECTED_MESSAGE_DETAILS](state) {
    clearSelectedMessageDetails(state);
  },
  [CLEAR_SELECTED_RECIPIENT](state) {
    state.selectedMessageRecipient = undefined;
  },
  [LOADED_MESSAGES](state, loaded) {
    state.loadedMessages = !!loaded;
  },
  [LOADED_RECIPIENTS](state, loaded) {
    state.loadedRecipients = !!loaded;
  },
  [MESSAGE_SENT](state, messageSent) {
    state.messageSent = messageSent;
  },
  [LOADED_MESSAGE](state, loaded) {
    state.loadedDetails = !!loaded;
  },
  [SET_SUMMARIES](state, data) {
    state.messageSummaries = data || [];
  },
  [SET_RECIPIENTS](state, data) {
    state.messageRecipients = data || [];
  },
  [SET_DETAILS](state, data) {
    state.selectedMessageDetails = data || undefined;

    if (state.selectedMessageDetails) {
      buildMessageMetadata(state);
    }
  },
  [SET_SELECTED_MESSAGE_ID](state, id) {
    state.selectedMessageId = id;
  },
  [SET_SELECTED_MESSAGE_RECIPIENT](state, recipient) {
    state.selectedMessageRecipient = recipient;
  },
  [SET_URGENCY_CHOICE](state, choice) {
    state.urgencyChoice = choice;
  },
  [SET_DELETED](state, status) {
    state.messageDeleted = status;
  },
  [SET_ATTACHMENT_ID](state, attachmentId) {
    state.attachmentId = attachmentId;
  },
};
