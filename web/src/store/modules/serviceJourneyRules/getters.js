import get from 'lodash/fp/get';
import { CDSS_ADMIN, CDSS_ADVICE, IM1_PROVIDER, INFORMATICA, NOMINATED_PHARMACY } from './mutation-types';

export default {
  [`${CDSS_ADMIN}Enabled`](state) {
    return !(state.rules.cdssAdmin.provider === 'none');
  },
  [`${CDSS_ADVICE}Enabled`](state) {
    return !(state.rules.cdssAdvice.provider === 'none');
  },
  [`${CDSS_ADMIN}Disabled`](state) {
    return state.rules.cdssAdmin.provider === 'none';
  },
  [`${CDSS_ADVICE}Disabled`](state) {
    return state.rules.cdssAdvice.provider === 'none';
  },
  [`${IM1_PROVIDER}Enabled`](state) {
    return get('rules.appointments.provider')(state) === IM1_PROVIDER;
  },
  [`${INFORMATICA}Enabled`](state) {
    return get('rules.appointments.provider')(state) === INFORMATICA;
  },
  [`${NOMINATED_PHARMACY}Enabled`](state) {
    return get('rules.nominatedPharmacy')(state);
  },
};
