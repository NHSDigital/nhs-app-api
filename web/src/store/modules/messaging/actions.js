import {
  LOADED,
  INIT,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  load({ commit }) {
    return this.app.$http
      .getV1ApiUsersMeMessages()
      .then((data) => {
        commit(LOADED, data);
      });
  },
};
