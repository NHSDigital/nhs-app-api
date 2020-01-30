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
  SET_STATUS_STATE,
  initialState,
  MESSAGE_SENT,
} from './mutation-types';

const clearMessage = (state) => {
  state.selectedMessageDetails = undefined;
  state.selectedMessageId = undefined;
  state.selectedMessageRecipient = undefined;
  state.statusState = undefined;
  state.messageSent = false;
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
  [LOADED_MESSAGES](state, loaded) {
    state.loadedMessages = !!loaded;
  },
  [LOADED_RECIPIENTS](state, loaded) {
    state.loadedRecipients = !!loaded;
  },
  [MESSAGE_SENT](state) {
    state.messageSent = true;
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
  [SET_STATUS_STATE](state, status) {
    state.statusState = status;
  },
};
