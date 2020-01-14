import get from 'lodash/fp/get';
import {
  INIT,
  LOADED,
  LOADED_MESSAGE,
  SET_SELECTED_MESSAGE_ID,
  CLEAR,
  SET_DETAILS,
  SET_SUMMARIES,
  SET_SELECTED_MESSAGE_RECIPIENT,
  SET_STATUS_STATE,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  load({ commit }, clearApiError) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');
      commit(LOADED, false);
    }
    return this.app.$http.getV1PatientMessages()
      .then((response) => {
        commit(SET_SUMMARIES, get('messageSummaries', response));
        commit(LOADED, true);
      }).catch(() => {});
  },
  clearErrorsAndLoad() {
    return this.dispatch('patientPracticeMessaging/load', true);
  },
  loadMessage({ commit }, { id, clearApiError }) {
    if (clearApiError) {
      this.dispatch('errors/clearAllApiErrors');
      commit(LOADED_MESSAGE, false);
    }
    const request = {
      id,
    };
    return this.app.$http
      .getV1PatientMessagesById(request)
      .then((data) => {
        commit(SET_DETAILS, data);
        commit(LOADED_MESSAGE, true);
      })
      .catch(() => {
      });
  },
  setSelectedMessageID({ commit }, id) {
    commit(SET_SELECTED_MESSAGE_ID, id);
  },
  setSelectedRecipient({ commit }, recipient) {
    commit(SET_SELECTED_MESSAGE_RECIPIENT, recipient);
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  clearErrorsAndLoadDetails({ state }) {
    return this.dispatch('patientPracticeMessaging/loadMessage', { id: state.selectedMessageId, clearApiError: true });
  },
  updateReadStatusAsRead({ commit, state }) {
    const request = {
      updateMessageReadStatusRequestBody: {
        messageId: state.selectedMessageId,
        messageReadState: 'Read',
      },
    };
    this.app.$http
      .postV1PatientMessagesUpdateReadStatus(request)
      .then((data) => {
        commit(SET_STATUS_STATE, data.messageReadStateUpdateStatus);
      })
      .catch(() => {
      });
  },
};
