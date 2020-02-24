import { INIT, LOADSERVICES } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const knownServiceList = await this.app.$httpV2.getV2Configuration();
    const services = knownServiceList.knownServices.map(({ url, requiresAssertedLoginIdentity }) =>
      ({ url, requiresAssertedLoginIdentity }));
    commit(LOADSERVICES, services);
    return true;
  },
};
