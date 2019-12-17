import {
  SET_ACCEPTANCE,
  INIT_ACCEPTANCE,
  SET_UPDATED_CONSENT_REQUIRED,
} from '@/store/modules/termsAndConditions/mutation-types';

export default {
  init({ commit }) {
    commit(INIT_ACCEPTANCE);
  },
  async toggleAnalyticsCookieConsent({ commit }, analyticsCookieConsent) {
    await this.app.$http
      .postV1PatientTermsAndConditionsToggleAnalyticsCookieAcceptance(analyticsCookieConsent)
      .then(() => {
        commit(SET_ACCEPTANCE, { analyticsCookieAccepted: false });
        return true;
      })
      .catch(() => false);
  },
  async acceptTerms({ commit }, consentTerms) {
    let analyticsCookie = false;
    if (consentTerms.consentRequest.UpdatingConsent) {
      await this
        .app
        .$http
        .getV1PatientTermsAndConditionsConsent({})
        .then((data) => {
          if (data) {
            analyticsCookie = data.response.analyticsCookieAccepted;
          }
        })
        .catch(err => Promise.reject(err));
    } else { analyticsCookie = consentTerms.consentRequest.AnalyticsCookieAccepted; }
    return this
      .app
      .$http
      .postV1PatientTermsAndConditionsConsent(consentTerms)
      .then(() => {
        const consentGiven = {
          areAccepted: consentTerms.consentRequest.ConsentGiven,
          analyticsCookieAccepted: analyticsCookie,
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
    const getTermsAndConditions = (termsAndConditions, property) =>
      termsAndConditions[property] || (this.app.$cookies.get('nhso.terms') || {})[property];
    const areAccepted = getTermsAndConditions(state, 'areAccepted');
    const updatedConsentRequired = getTermsAndConditions(state, 'updatedConsentRequired');
    const analyticsCookieAccepted = getTermsAndConditions(state, 'analyticsCookieAccepted') || false;

    let promise;
    if (areAccepted && !updatedConsentRequired) {
      promise = Promise.resolve({
        consentGiven: {
          areAccepted,
          analyticsCookieAccepted,
        },
        consentRequired: updatedConsentRequired,
      });
    } else {
      promise = this
        .app
        .$http
        .getV1PatientTermsAndConditionsConsent({})
        .then((data) => {
          if (data) {
            const consentGiven = {
              areAccepted: data.response.consentGiven,
              analyticsCookieAccepted: data.response.analyticsCookieAccepted,
            };
            return Promise.resolve({
              consentGiven,
              consentRequired: data.response.updatedConsentRequired,
            });
          }
          throw new Error('No T&C response');
        })
        .catch(err => Promise.reject(err));
    }

    return promise.then(({ consentGiven, consentRequired }) => {
      commit(SET_ACCEPTANCE, consentGiven);
      commit(SET_UPDATED_CONSENT_REQUIRED, consentRequired);
    });
  },
};
