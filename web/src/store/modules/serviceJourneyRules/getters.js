import get from 'lodash/fp/get';
import { getterNames, mutationNames } from './constants';

const {
  ONLINE_CONSULTATIONS_ENABLED,
  CDSS_ADMIN_ENABLED,
  CDSS_ADVICE_ENABLED,
  GP_AT_HAND_APPOINTMENTS_ENABLED,
  IM1_PROVIDER_APPOINTMENTS_ENABLED,
  INFORMATICA_APPOINTMENTS_ENABLED,
  LINKED_ACCOUNT_APPOINTMENTS_ENABLED,
  GP_AT_HAND_MY_RECORD_ENABLED,
  IM1_PROVIDER_MY_RECORD_ENABLED,
  GP_AT_HAND_PRESCRIPTIONS_ENABLED,
  IM1_PROVIDER_PRESCRIPTIONS_ENABLED,
  GP_MEDICAL_RECORD_V1_ENABLED,
  GP_MEDICAL_RECORD_V2_ENABLED,
  GP_AT_HAND_GP_MEDICAL_RECORD_V2_ENABLED,
  IM1_PROVIDER_GP_MEDICAL_RECORD_V2_ENABLED,
  MESSAGING_ENABLED,
  NOMINATED_PHARMACY_ENABLED,
  NOTIFICATIONS_ENABLED,
  SILVER_INTEGRATION_ENABLED,
  DOCUMENTS_ENABLED,
  IM1MESSAGING_ENABLED,
  DELETE_PATIENT_PRACTICE_MESSAGING_ENABLED,
  REQUIRED_DETAILS_CALL_PATIENT_PRACTICE_MESSAGING_ENABLED,
  UPDATE_STATUS_PATIENT_PRACTICE_MESSAGING_ENABLED,
  SEND_MESSAGE_SUBJECT_ENABLED,
} = getterNames;

const {
  GP_AT_HAND,
  IM1_PROVIDER,
  INFORMATICA,
  LINKED_ACCOUNT,
} = mutationNames;

// export references getterNames to allow sharing of
// names between eslint and this code
export default {
  [ONLINE_CONSULTATIONS_ENABLED](_, getters) {
    return getters[CDSS_ADMIN_ENABLED] || getters[CDSS_ADVICE_ENABLED];
  },
  [CDSS_ADMIN_ENABLED](state) {
    return get('rules.cdssAdmin.provider')(state) !== 'none';
  },
  [CDSS_ADVICE_ENABLED](state) {
    return get('rules.cdssAdvice.provider')(state) !== 'none';
  },
  [GP_AT_HAND_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === GP_AT_HAND;
  },
  [IM1_PROVIDER_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === IM1_PROVIDER;
  },
  [INFORMATICA_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === INFORMATICA;
  },
  [LINKED_ACCOUNT_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === LINKED_ACCOUNT;
  },
  [GP_AT_HAND_MY_RECORD_ENABLED](state) {
    return get('rules.medicalRecord.provider')(state) === GP_AT_HAND;
  },
  [IM1_PROVIDER_MY_RECORD_ENABLED](state) {
    return get('rules.medicalRecord.provider')(state) === IM1_PROVIDER;
  },
  [GP_AT_HAND_PRESCRIPTIONS_ENABLED](state) {
    return get('rules.prescriptions.provider')(state) === GP_AT_HAND;
  },
  [IM1_PROVIDER_PRESCRIPTIONS_ENABLED](state) {
    return get('rules.prescriptions.provider')(state) === IM1_PROVIDER;
  },
  [GP_MEDICAL_RECORD_V1_ENABLED](state) {
    return get('rules.medicalRecord.version')(state) === '1';
  },
  [GP_MEDICAL_RECORD_V2_ENABLED](state) {
    return get('rules.medicalRecord.version')(state) === '2';
  },
  [GP_AT_HAND_GP_MEDICAL_RECORD_V2_ENABLED](_, getters) {
    return getters[GP_MEDICAL_RECORD_V2_ENABLED] && getters[GP_AT_HAND_MY_RECORD_ENABLED];
  },
  [IM1_PROVIDER_GP_MEDICAL_RECORD_V2_ENABLED](_, getters) {
    return getters[GP_MEDICAL_RECORD_V2_ENABLED] && getters[IM1_PROVIDER_MY_RECORD_ENABLED];
  },
  [MESSAGING_ENABLED](state) {
    return get('rules.messaging')(state);
  },
  [NOMINATED_PHARMACY_ENABLED](state) {
    return get('rules.nominatedPharmacy')(state);
  },
  [NOTIFICATIONS_ENABLED](state) {
    return get('rules.notifications')(state);
  },
  [SILVER_INTEGRATION_ENABLED](state) {
    return ({ provider, serviceType }) => get(`rules.silverIntegrations.${serviceType}`)(state).includes(provider);
  },
  [DOCUMENTS_ENABLED](state) {
    return get('rules.documents')(state);
  },
  [IM1MESSAGING_ENABLED](state) {
    return get('rules.im1Messaging.isEnabled')(state);
  },
  [DELETE_PATIENT_PRACTICE_MESSAGING_ENABLED](state) {
    return get('rules.im1Messaging.canDeleteMessages')(state);
  },
  [UPDATE_STATUS_PATIENT_PRACTICE_MESSAGING_ENABLED](state) {
    return get('rules.im1Messaging.canUpdateReadStatus')(state);
  },
  [REQUIRED_DETAILS_CALL_PATIENT_PRACTICE_MESSAGING_ENABLED](state) {
    return get('rules.im1Messaging.requiresDetailsRequest')(state);
  },
  [SEND_MESSAGE_SUBJECT_ENABLED](state) {
    return get('rules.im1Messaging.sendMessageSubject')(state);
  },
};
