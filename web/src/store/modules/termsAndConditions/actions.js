import getOr from 'lodash/fp/getOr';
import { INDEX } from '@/lib/routes';
import { SET_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';

const setAcceptance = (app, commit, consentTerms) => {
  const cookie = app.$cookies.get('nhso.session');
  if (cookie) {
    cookie.termsAccepted = consentTerms;
    app.$cookies.set('nhso.session', cookie);
  }

  commit(SET_ACCEPTANCE, consentTerms);
};

const extractConsentGiven = getOr(false, 'response.consentGiven');

export default {
  async acceptTerms({ commit }, consentTerms) {
    return this
      .app
      .$http
      .postV1PatientTermsAndConditionsConsent(consentTerms)
      .then(() => {
        setAcceptance(this.app, commit, true);
        this.app.router.push(INDEX.path);
      })
      .catch(() => setAcceptance(this.app, commit, false));
  },
  async checkAcceptance({ commit, state }) {
    if (state.areAccepted) return Promise.resolve();
    return this
      .app
      .$http
      .getV1PatientTermsAndConditionsConsent({})
      .then((data) => {
        const consentGiven = extractConsentGiven(data);
        setAcceptance(this.app, commit, consentGiven);
        return Promise.resolve();
      })
      .catch(err => Promise.reject(err));
  },

  setAcceptance({ commit }, consentTerms) {
    return setAcceptance(this.app, commit, consentTerms);
  },
};
