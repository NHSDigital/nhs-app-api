import { ONLINE_CONSULTATIONS } from './mutation-types';

export default {
  [`${ONLINE_CONSULTATIONS}Enabled`](state) {
    return state.rules.onlineConsultations;
  },
};
