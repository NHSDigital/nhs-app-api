import get from 'lodash/fp/get';
import {
  CDSS_ADMIN,
  CDSS_ADVICE,
  GP_AT_HAND,
  GP_MEDICAL_RECORD,
  IM1_PROVIDER,
  INFORMATICA,
  MESSAGING,
  NOMINATED_PHARMACY,
  NOTIFICATIONS,
  ONLINE_CONSULTATIONS,
  LINKED_ACCOUNTS,
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
  [`${GP_MEDICAL_RECORD}V1Enabled`](state) {
    return get('rules.medicalRecord.version')(state) === 1;
  },
  [`${GP_MEDICAL_RECORD}V2Enabled`](state) {
    return get('rules.medicalRecord.version')(state) === 2;
  },
  [`${GP_AT_HAND}GpMedicalRecordV2Enabled`](_, getters) {
    return getters[`${GP_MEDICAL_RECORD}V2Enabled`] && getters[`${GP_AT_HAND}MyRecordEnabled`];
  },
  [`${IM1_PROVIDER}GpMedicalRecordV2Enabled`](_, getters) {
    return getters[`${GP_MEDICAL_RECORD}V2Enabled`] && getters[`${IM1_PROVIDER}MyRecordEnabled`];
  },
  [`${MESSAGING}Enabled`](state) {
    return get('rules.messaging')(state);
  },
  [`${NOMINATED_PHARMACY}Enabled`](state) {
    return get('rules.nominatedPharmacy')(state);
  },
  [`${NOTIFICATIONS}Enabled`](state) {
    return get('rules.notifications')(state);
  },
  [`${LINKED_ACCOUNTS}Enabled`](state) {
    return get('rules.hasLinkedAccounts')(state);
  },
};
