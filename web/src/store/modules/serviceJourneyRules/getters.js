import get from 'lodash/fp/get';
import { CDSS_ADMIN,
  CDSS_ADVICE,
  GP_AT_HAND,
  IM1_PROVIDER,
  INFORMATICA,
  NOMINATED_PHARMACY,
  NOTIFICATIONS,
  ONLINE_CONSULTATIONS,
} from './mutation-types';

export default {
  [`${ONLINE_CONSULTATIONS}Enabled`](_, getters) {
    return getters[`${CDSS_ADMIN}Enabled`] || getters[`${CDSS_ADVICE}Enabled`];
  },
  [`${CDSS_ADMIN}Enabled`](state) {
    return get('rules.cdssAdmin.provider')(state) !== 'none';
  },
  [`${CDSS_ADVICE}Enabled`](state) {
    return get('rules.cdssAdvice.provider')(state) !== 'none';
  },
  [`${GP_AT_HAND}AppointmentsEnabled`](state) {
    return get('rules.appointments.provider')(state) === GP_AT_HAND;
  },
  [`${IM1_PROVIDER}AppointmentsEnabled`](state) {
    return get('rules.appointments.provider')(state) === IM1_PROVIDER;
  },
  [`${INFORMATICA}AppointmentsEnabled`](state) {
    return get('rules.appointments.provider')(state) === INFORMATICA;
  },
  [`${GP_AT_HAND}MyRecordEnabled`](state) {
    return get('rules.medicalRecord.provider')(state) === GP_AT_HAND;
  },
  [`${IM1_PROVIDER}MyRecordEnabled`](state) {
    return get('rules.medicalRecord.provider')(state) === IM1_PROVIDER;
  },
  [`${GP_AT_HAND}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === GP_AT_HAND;
  },
  [`${IM1_PROVIDER}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === IM1_PROVIDER;
  },
  [`${GP_AT_HAND}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === GP_AT_HAND;
  },
  [`${IM1_PROVIDER}PrescriptionsEnabled`](state) {
    return get('rules.prescriptions.provider')(state) === IM1_PROVIDER;
  },
  [`${NOMINATED_PHARMACY}Enabled`](state) {
    return get('rules.nominatedPharmacy')(state);
  },
  [`${NOTIFICATIONS}Enabled`](state) {
    return get('rules.notifications')(state);
  },
};
