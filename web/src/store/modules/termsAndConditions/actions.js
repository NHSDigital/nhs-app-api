import getOr from 'lodash/fp/getOr';
import { SET_ACCEPTANCE, INIT_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';

const extractConsentGiven = getOr(false, 'response.consentGiven');

export default {
  init({ commit }) {
    commit(INIT_ACCEPTANCE);
  },
  async acceptTerms({ commit }, consentTerms) {
    return this
      .app
      .$http
      .postV1PatientTermsAndConditionsConsent(consentTerms)
      .then(() => {
        commit(SET_ACCEPTANCE, consentTerms);
        return Promise.resolve();
      })
      .catch(() => {
        commit(SET_ACCEPTANCE, false);
        return Promise.resolve();
      });
  },
  async checkAcceptance({ commit, state }) {
    if (state.areAccepted) return Promise.resolve();
    return this
      .app
      .$http
      .getV1PatientTermsAndConditionsConsent({})
      .then((data) => {
        const consentGiven = extractConsentGiven(data);
        commit(SET_ACCEPTANCE, consentGiven);
        return Promise.resolve();
      })
      .catch(err => Promise.reject(err));
  },
};
