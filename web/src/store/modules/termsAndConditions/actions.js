import getOr from 'lodash/fp/getOr';
import { INDEX } from '@/lib/routes';
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
        const sourceValue = this.app.store.state.device.source;
        this.app.router.push({
          path: INDEX.path,
          query: { source: sourceValue },
        });
      })
      .catch(() => commit(SET_ACCEPTANCE, false));
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
