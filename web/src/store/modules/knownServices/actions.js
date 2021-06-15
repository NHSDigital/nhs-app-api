import { INIT, LOADSERVICES } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const knownServiceList = await this.app.$httpV3.getV3KnownServices();
    const services = knownServiceList.knownServices.map((
      { id, url, showThirdPartyWarning, requiresAssertedLoginIdentity, domains },
    ) =>
      ({ id, url, showThirdPartyWarning, requiresAssertedLoginIdentity, domains }));
    commit(LOADSERVICES, services);
    return true;
  },
};
