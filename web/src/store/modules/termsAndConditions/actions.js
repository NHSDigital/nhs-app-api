import getOr from 'lodash/fp/getOr';
import { SET_ACCEPTANCE, INIT_ACCEPTANCE, SET_UPDATED_CONSENT_REQUIRED } from '@/store/modules/termsAndConditions/mutation-types';

const extractConsentGiven = getOr(false, 'response.consentGiven');
const extractUpdatedConsentRequired = getOr(false, 'response.updatedConsentRequired');

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
    if (state.areAccepted && !state.updatedConsentRequired) return Promise.resolve();
    return this
      .app
      .$http
      .getV1PatientTermsAndConditionsConsent({})
      .then((data) => {
        const consentGiven = extractConsentGiven(data);
        const updatedConsentRequired = extractUpdatedConsentRequired(data);
        commit(SET_ACCEPTANCE, consentGiven);
        commit(SET_UPDATED_CONSENT_REQUIRED, updatedConsentRequired);
        return Promise.resolve();
      })
      .catch(err => Promise.reject(err));
  },
};
