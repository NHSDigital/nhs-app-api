import get from 'lodash/fp/get';
import { INFORMATICA, IM1_PROVIDER, CDSS_ADMIN, CDSS_ADVICE } from './mutation-types';

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
};
