import { createLocalError } from '@/lib/utils';
import {
  ADD_ERROR,
  CLEAR,
  INIT,
  LOADED,
  SENDER_MESSAGES_LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  SET_HAS_UNREAD,
} from './mutation-types';

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
  async load({ commit }, request = {}) {
    // eslint-disable-next-line no-param-reassign
    request.ignoreError = true;

    try {
      const data = await this.app.$http.getV1ApiUsersMeMessages(request);
      commit(LOADED, data);
      commit(SET_HAS_UNREAD, data);
    } catch (error) {
      commit(ADD_ERROR, createLocalError(error));
    } finally {
      commit(SENDER_MESSAGES_LOADED);
      this.dispatch('device/unlockNavBar');
    }
  },
  async loadSenders({ commit }) {
    try {
      const { senders } = this.$env.MESSAGES_SENDER_ID_ENABLED
        ? await this.app.$httpV2.getV2ApiUsersMeMessagesSenders({ ignoreError: true })
        : await this.app.$http.getV1ApiUsersMeMessagesSenders({ ignoreError: true });

      commit(LOADED_SENDERS, senders);
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
  markAsRead(_, messageId) {
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
    this.app.$http.patchV1ApiUsersMeMessagesByMessageid(
      request,
    );
  },
};
