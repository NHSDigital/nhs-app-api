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
};
