import { SET_ACCEPTANCE, INIT_ACCEPTANCE, SET_UPDATED_CONSENT_REQUIRED } from '@/store/modules/termsAndConditions/mutation-types';

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
        const consentGiven = {
          areAccepted: consentTerms.consentRequest.ConsentGiven,
          analyticsCookieAccepted: consentTerms.consentRequest.AnalyticsCookieAccepted,
        };

        commit(SET_ACCEPTANCE, consentGiven);
        commit(SET_UPDATED_CONSENT_REQUIRED, false);
        return Promise.resolve();
      })
      .catch(() => {
        commit(SET_ACCEPTANCE, { areAccepted: false, analyticsCookieAccepted: false });
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
        const consentGiven = {
          areAccepted: data.response.consentGiven,
          analyticsCookieAccepted: data.response.analyticsCookieAccepted,
        };
        commit(SET_ACCEPTANCE, consentGiven);
        commit(SET_UPDATED_CONSENT_REQUIRED, data.response.updatedConsentRequired);
        return Promise.resolve();
      })
      .catch(err => Promise.reject(err));
  },
};
