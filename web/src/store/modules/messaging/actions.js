import { createLocalError } from '@/lib/utils';
import {
  ADD_ERROR,
  ADD_ERROR_REPLY,
  CLEAR,
  INIT,
  LOADED,
  SENDER_MESSAGES_LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  SET_HAS_UNREAD,
  SET_UNREADMESSAGE_SENDER_COUNT,
  DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT,
  SET_PREVIOUS_CHOICE,
  CLEAR_ERROR_REPLY,
  CLEAR_ALL_EXCEPT_MESSAGE_OBJ,
} from './mutation-types';
import NativeApp from '@/services/native-app';

export default {
  init({ commit }) {
    commit(INIT);
  },
  linkClicked(_, { messageId, link }) {
    const request = {
      messageId,
      linkClickedMessage: {
        link,
      },
      ignoreError: true,
      ignoreLoading: true,
    };
    this.app.$http.postV1ApiUsersMeMessagesByMessageidLinkClickedMetrics(
      request,
    );
  },
  clear({ commit }) {
    commit(CLEAR);
  },
  clearErrorReply({ commit }) {
    commit(CLEAR_ERROR_REPLY);
  },
  clearAllButMessage({ commit }) {
    commit(CLEAR_ALL_EXCEPT_MESSAGE_OBJ);
  },
  async load({ commit }, request = {}) {
    // eslint-disable-next-line no-param-reassign
    request.ignoreError = true;

    try {
      const data = await this.app.$http.getV1ApiUsersMeMessages(request);
      commit(LOADED, data);
      commit(SET_HAS_UNREAD, data);
      if (request.summary) {
        commit(SET_UNREADMESSAGE_SENDER_COUNT, data || []);
        this.dispatch('messaging/setBadgeCount');
      }
    } catch (error) {
      commit(ADD_ERROR, createLocalError(error));
    } finally {
      commit(SENDER_MESSAGES_LOADED);
      this.dispatch('device/unlockNavBar');
    }
  },
  async loadSenders({ commit }) {
    try {
      const { senders } = await this.app.$httpV2.getV2ApiUsersMeMessagesSenders({
        ignoreError: true,
      });

      commit(LOADED_SENDERS, senders);
      commit(SET_UNREADMESSAGE_SENDER_COUNT, senders);
      this.dispatch('messaging/setBadgeCount');
    } catch (error) {
      commit(ADD_ERROR, createLocalError(error));
    }
  },
  async loadMessage({ commit }, { messageId }) {
    try {
      const message = await this.app.$http.getV1ApiUsersMeMessagesByMessageid({
        messageId,
        ignoreError: true,
      });

      commit(LOADED_MESSAGE, message);
    } catch (error) {
      commit(ADD_ERROR, createLocalError(error));
    }
  },
  async markAsRead({ commit }, messageId) {
    const request = {
      messageId,
      patchMessageRequest: [{
        op: 'add',
        path: '/read',
        value: true,
      }],
      ignoreError: true,
      ignoreLoading: true,
    };
    await this.app.$http.patchV1ApiUsersMeMessagesByMessageid(request);

    commit(DECREMENT_TOTAL_UNREAD_MESSAGE_COUNT);
    this.dispatch('messaging/setBadgeCount');
  },
  async recordMessageResponse({ commit }, { messageId, response }) {
    const request = {
      messageId,
      patchMessageRequest: [{
        op: 'add',
        path: '/reply/response',
        value: response,
      }],
      ignoreError: true,
      ignoreLoading: true,
    };
    try {
      await this.app.$http.patchV1ApiUsersMeMessagesByMessageid(
        request,
      );
    } catch (error) {
      commit(ADD_ERROR_REPLY, createLocalError(error));
    }
  },
  addErrorReply({ commit }, message) {
    commit(ADD_ERROR_REPLY, message);
  },
  setPreviousChoice({ commit }, choice) {
    commit(SET_PREVIOUS_CHOICE, choice);
  },
  setBadgeCount({ state }) {
    if (this.$env.IOS_BADGE_COUNT_ENABLED) {
      NativeApp.setBadgeCount(state.totalUnreadMessageCount);
    }
  },
};
