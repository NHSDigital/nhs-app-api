import {
  INIT,
  LOADED,
  SET_SENDER,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  load({ commit }, sender) {
    const request = sender ? { sender } : { summary: true };

    return this.app.$http
      .getV1ApiUsersMeMessages(request)
      .then((data) => {
        commit(LOADED, data);
      })
      .finally(() => {
        this.dispatch('device/unlockNavBar');
      });
  },
  selectSender({ commit }, sender) {
    commit(SET_SENDER, sender);
  },
  // eslint-disable-next-line no-unused-vars
  markAsRead({ commit }, messageId) {
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
