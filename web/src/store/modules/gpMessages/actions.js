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
  SET_URGENCY_CHOICE,
  MESSAGE_SENT,
  SET_DELETED,
  CLEAR_SELECTED_MESSAGE_DETAILS,
  CLEAR_SELECTED_RECIPIENT,
  SET_ATTACHMENT_ID,
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
    return this.dispatch('gpMessages/loadMessages', true);
  },
  async loadRecipients({ commit }, clearApiError) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');

      commit(LOADED_RECIPIENTS, false);
    }

    commit(CLEAR);

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
    return this.dispatch('gpMessages/loadMessage', { id: state.selectedMessageId, clearApiError: true });
  },
  retryMessageDelete({ state }) {
    this.dispatch('gpMessages/deleteMessage', state.selectedMessageId);
  },
  async updateReadStatusAsRead({ state }) {
    const request = {
      updateMessageReadStatusRequestBody: {
        messageId: state.selectedMessageId,
        messageReadState: 'Read',
      },
    };

    try {
      await this.app.$http.putV1PatientMessagesUpdateReadStatus(request);
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  async sendMessage({ commit, state }, message) {
    const createMessageRequest = {
      messageBody: message.messageText,
      subject: message.subjectText,
      recipientIdentifier: state.selectedMessageRecipient.id,
    };

    try {
      const response = await this.app.$http.postV1PatientMessages({
        createMessageRequest,
        returnResponse: true,
      });

      if (response.status === 204) {
        commit(MESSAGE_SENT, true);
      }
    } catch {
      // Nothing to do. A server error / messages error is displayed
    }
  },
  async deleteMessage({ commit }, id) {
    try {
      await this.app.$http.deleteV1PatientMessagesById({ id });

      commit(SET_DELETED, true);
    } catch {
      commit(SET_DELETED, false);
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
  setAttachmentId({ commit }, id) {
    commit(SET_ATTACHMENT_ID, id);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  clearSelectedRetainingId({ commit }) {
    commit(CLEAR_SELECTED_MESSAGE_DETAILS);
  },
  clearSelectedRecipient({ commit }) {
    commit(CLEAR_SELECTED_RECIPIENT);
  },
  setMessageSent({ commit }, messageSent) {
    commit(MESSAGE_SENT, messageSent);
  },
};
