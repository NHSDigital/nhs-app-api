import get from 'lodash/fp/get';
import { INFORMATICA, IM1_PROVIDER, ONLINE_CONSULTATIONS } from './mutation-types';

export default {
  [`${ONLINE_CONSULTATIONS}Enabled`](state) {
    return state.rules.onlineConsultations;
  },
  [`${IM1_PROVIDER}Enabled`](state) {
    return get('rules.appointments.provider')(state) === IM1_PROVIDER;
  },
  [`${INFORMATICA}Enabled`](state) {
    return get('rules.appointments.provider')(state) === INFORMATICA;
  },
};
