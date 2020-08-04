import { INIT, LOADSERVICES } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const knownServiceList = await this.app.$httpV2.getV2Configuration();
    const services = knownServiceList.knownServices.map((
      { id, url, showThirdPartyWarning, requiresAssertedLoginIdentity },
    ) =>
      ({ id, url, showThirdPartyWarning, requiresAssertedLoginIdentity }));
    commit(LOADSERVICES, services);
    return true;
  },
};
