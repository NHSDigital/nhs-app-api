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
  LINKED_ACCOUNT,
  SILVER_INTEGRATION,
  DOCUMENTS,
  IM1MESSAGING,
  DELETE_PATIENT_PRACTICE_MESSAGING,
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
  [`${LINKED_ACCOUNT}AppointmentsEnabled`](state) {
    return get('rules.appointments.provider')(state) === LINKED_ACCOUNT;
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
    return get('rules.medicalRecord.version')(state) === '1';
  },
  [`${GP_MEDICAL_RECORD}V2Enabled`](state) {
    return get('rules.medicalRecord.version')(state) === '2';
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
  [`${SILVER_INTEGRATION}Enabled`](state) {
    return ({ provider, serviceType }) => get(`rules.silverIntegrations.${serviceType}`)(state).includes(provider);
  },
  [`${DOCUMENTS}Enabled`](state) {
    return get('rules.documents')(state);
  },
  [`${IM1MESSAGING}Enabled`](state) {
    return get('rules.im1Messaging.isEnabled')(state);
  },
  [`${DELETE_PATIENT_PRACTICE_MESSAGING}Enabled`](state) {
    return get('rules.im1Messaging.canDeleteMessages')(state);
  },
};
