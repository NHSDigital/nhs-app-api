import { INIT, SET_RULES, SET_ADMIN_PROVIDER_NAME, SET_ADVICE_PROVIDER_NAME } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT);
  },
  async load({ commit }) {
    const response = await this.app.$http.getV1PatientJourneyConfiguration();
    if (response) {
      commit(SET_RULES, response);
    }

    const adminName = response.journeys.cdssAdmin.provider;
    await this.app.$cdsApi.getFhirServiceDefinitionProviderNameByProvider({
      provider: adminName,
    }).then((providerName) => {
      commit(SET_ADMIN_PROVIDER_NAME, providerName);
    });

    const adviceName = response.journeys.cdssAdvice.provider;
    await this.app.$cdsApi.getFhirServiceDefinitionProviderNameByProvider({
      provider: adviceName,
    }).then((providerName) => {
      commit(SET_ADVICE_PROVIDER_NAME, providerName);
    });
  },

};
