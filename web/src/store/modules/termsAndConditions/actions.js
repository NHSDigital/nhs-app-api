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

    const {
      response: { consentGiven = true },
    } = await this.app.$http.getV1PatientTermsAndConditionsConsent({});
    setAcceptance(this.app, commit, consentGiven);
    return Promise.resolve();
  },

  setAcceptance({ commit }, consentTerms) {
    return setAcceptance(this.app, commit, consentTerms);
  },
};
