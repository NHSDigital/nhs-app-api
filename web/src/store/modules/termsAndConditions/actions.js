import {
  SET_ACCEPTANCE,
  INIT_ACCEPTANCE,
  SET_UPDATED_CONSENT_REQUIRED,
} from '@/store/modules/termsAndConditions/mutation-types';

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
    const getTermsAndConditions = (termsAndConditions, property) =>
      termsAndConditions[property] || (this.app.$cookies.get('nhso.terms') || {})[property];

    const areAccepted = getTermsAndConditions(state, 'areAccepted');
    const updatedConsentRequired = getTermsAndConditions(state, 'updatedConsentRequired');

    let promise;
    if (areAccepted && !updatedConsentRequired) {
      promise = Promise.resolve({
        consentGiven: {
          areAccepted,
          analyticsCookieAccepted: state.analyticsCookieAccepted,
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
