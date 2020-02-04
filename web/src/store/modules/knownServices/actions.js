import { INIT, LOADSERVICES } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const knownServiceList = await this.app.$httpV2.getV2Configuration();
    const urls = knownServiceList.knownServices.map(({ url }) => url);
    commit(LOADSERVICES, urls);
    return true;
  },
};
