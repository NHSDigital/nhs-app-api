import {
  INIT,
  LOADED,
  SET_SENDER,
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
  async load({ commit }, { sender, ignoreError } = {}) {
    const request = sender ? { sender } : { summary: true };
    request.ignoreError = ignoreError;

    try {
      const data = await this.app.$http.getV1ApiUsersMeMessages(request);
      commit(LOADED, data);
      commit(SET_HAS_UNREAD, data);
    } catch (error) {
      if (!ignoreError) {
        throw error;
      }
    } finally {
      this.dispatch('device/unlockNavBar');
    }
  },
  selectSender({ commit }, sender) {
    commit(SET_SENDER, sender);
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
