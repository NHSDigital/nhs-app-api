import get from 'lodash/fp/get';
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
  SET_STATUS_STATE,
  SET_URGENCY_CHOICE,
  MESSAGE_SENT,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async loadMessages({ commit }, clearApiError) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');
      commit(LOADED_MESSAGES, false);
    }
    try {
      const response = await this.app.$http.getV1PatientMessages();
      commit(SET_SUMMARIES, get('messageSummaries', response));
      commit(LOADED_MESSAGES, true);
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  clearErrorsAndLoadMessages() {
    return this.dispatch('patientPracticeMessaging/loadMessages', true);
  },
  async loadRecipients({ commit }, clearApiError) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');
      commit(LOADED_RECIPIENTS, false);
    }
    try {
      const response = await this.app.$http.getV1PatientMessagesRecipients();
      commit(SET_RECIPIENTS, get('messageRecipients', response));
      commit(LOADED_RECIPIENTS, true);
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  async loadMessage({ commit }, { id, clearApiError }) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');
      commit(LOADED_MESSAGE, false);
    }
    try {
      const response = await this.app.$http.getV1PatientMessagesById({ id });
      commit(SET_DETAILS, response);
      commit(LOADED_MESSAGE, true);
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  clearErrorsAndLoadDetails({ state }) {
    return this.dispatch('patientPracticeMessaging/loadMessage', { id: state.selectedMessageId, clearApiError: true });
  },
  async updateReadStatusAsRead({ commit, state }) {
    const request = {
      updateMessageReadStatusRequestBody: {
        messageId: state.selectedMessageId,
        messageReadState: 'Read',
      },
    };
    try {
      const response = await this.app.$http.postV1PatientMessagesUpdateReadStatus(request);
      commit(SET_STATUS_STATE, response.messageReadStateUpdateStatus);
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  async sendMessage({ commit, state }, message) {
    const createMessageRequest = {
      messageBody: message.messageText,
      subject: message.subjectText,
      recipient: state.selectedMessageRecipient.id,
    };
    try {
      const response = await this.app.$http.postV1PatientMessages({ createMessageRequest });
      if (response.messageSent) {
        commit(MESSAGE_SENT);
      }
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  setMessageDetails({ commit }, messageDetails) {
    commit(SET_DETAILS, messageDetails);
    commit(LOADED_MESSAGE, true);
  },
  setSelectedMessageID({ commit }, id) {
    commit(SET_SELECTED_MESSAGE_ID, id);
  },
  setSelectedRecipient({ commit }, recipient) {
    commit(SET_SELECTED_MESSAGE_RECIPIENT, recipient);
  },
  setUrgencyChoice({ commit }, choice) {
    commit(SET_URGENCY_CHOICE, choice);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
};
