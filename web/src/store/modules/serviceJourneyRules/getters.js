import { get, getOr } from 'lodash/fp';
import { getterNames, mutationNames } from './constants';

const {
  CDSS_ADMIN_ENABLED,
  CDSS_ADVICE_ENABLED,
  DELETE_GP_MESSAGES_ENABLED,
  DOCUMENTS_ENABLED,
  GP_AT_HAND_APPOINTMENTS_ENABLED,
  GP_AT_HAND_GP_MEDICAL_RECORD_V2_ENABLED,
  GP_AT_HAND_MY_RECORD_ENABLED,
  GP_AT_HAND_PRESCRIPTIONS_ENABLED,
  GP_MEDICAL_RECORD_V1_ENABLED,
  GP_MEDICAL_RECORD_V2_ENABLED,
  IM1MESSAGING_ENABLED,
  IM1_PROVIDER_APPOINTMENTS_ENABLED,
  IM1_PROVIDER_GP_MEDICAL_RECORD_V2_ENABLED,
  IM1_PROVIDER_MY_RECORD_ENABLED,
  IM1_PROVIDER_PRESCRIPTIONS_ENABLED,
  INFORMATICA_APPOINTMENTS_ENABLED,
  LINKED_ACCOUNT_APPOINTMENTS_ENABLED,
  MESSAGING_ENABLED,
  NOMINATED_PHARMACY_ENABLED,
  NOTIFICATION_PROMPT_ENABLED,
  NOTIFICATIONS_ENABLED,
  ONLINE_CONSULTATIONS_ENABLED,
  REQUIRED_DETAILS_CALL_GP_MESSAGES_ENABLED,
  SEND_MESSAGE_SUBJECT_ENABLED,
  SILVER_INTEGRATION_ENABLED,
  SILVER_INTEGRATION_APPOINTMENTS_ENABLED,
  SILVER_INTEGRATION_MESSAGES_ENABLED,
  UPDATE_STATUS_GP_MESSAGES_ENABLED,
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
  [CDSS_ADMIN_ENABLED](state) {
    return get('rules.cdssAdmin.provider')(state) !== 'none';
  },
  [CDSS_ADVICE_ENABLED](state) {
    return get('rules.cdssAdvice.provider')(state) !== 'none';
  },
  [DELETE_GP_MESSAGES_ENABLED](state) {
    return get('rules.im1Messaging.canDeleteMessages')(state);
  },
  [DOCUMENTS_ENABLED](state) {
    return get('rules.documents')(state);
  },
  [GP_AT_HAND_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === GP_AT_HAND;
  },
  [GP_AT_HAND_GP_MEDICAL_RECORD_V2_ENABLED](_, getters) {
    return getters[GP_MEDICAL_RECORD_V2_ENABLED] && getters[GP_AT_HAND_MY_RECORD_ENABLED];
  },
  [GP_AT_HAND_MY_RECORD_ENABLED](state) {
    return get('rules.medicalRecord.provider')(state) === GP_AT_HAND;
  },
  [GP_AT_HAND_PRESCRIPTIONS_ENABLED](state) {
    return get('rules.prescriptions.provider')(state) === GP_AT_HAND;
  },
  [GP_MEDICAL_RECORD_V1_ENABLED](state) {
    return get('rules.medicalRecord.version')(state) === '1';
  },
  [GP_MEDICAL_RECORD_V2_ENABLED](state) {
    return get('rules.medicalRecord.version')(state) === '2';
  },
  [IM1MESSAGING_ENABLED](state) {
    return get('rules.im1Messaging.isEnabled')(state);
  },
  [IM1_PROVIDER_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === IM1_PROVIDER;
  },
  [IM1_PROVIDER_GP_MEDICAL_RECORD_V2_ENABLED](_, getters) {
    return getters[GP_MEDICAL_RECORD_V2_ENABLED] && getters[IM1_PROVIDER_MY_RECORD_ENABLED];
  },
  [IM1_PROVIDER_MY_RECORD_ENABLED](state) {
    return get('rules.medicalRecord.provider')(state) === IM1_PROVIDER;
  },
  [IM1_PROVIDER_PRESCRIPTIONS_ENABLED](state) {
    return get('rules.prescriptions.provider')(state) === IM1_PROVIDER;
  },
  [INFORMATICA_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === INFORMATICA;
  },
  [LINKED_ACCOUNT_APPOINTMENTS_ENABLED](state) {
    return get('rules.appointments.provider')(state) === LINKED_ACCOUNT;
  },
  [MESSAGING_ENABLED](state) {
    return get('rules.messaging')(state);
  },
  [NOMINATED_PHARMACY_ENABLED](state) {
    return get('rules.nominatedPharmacy')(state);
  },
  [NOTIFICATION_PROMPT_ENABLED](state) {
    return get('rules.notificationPrompt')(state);
  },
  [NOTIFICATIONS_ENABLED](state) {
    return get('rules.notifications')(state);
  },
  [ONLINE_CONSULTATIONS_ENABLED](_, getters) {
    return getters[CDSS_ADMIN_ENABLED] || getters[CDSS_ADVICE_ENABLED];
  },
  [REQUIRED_DETAILS_CALL_GP_MESSAGES_ENABLED](state) {
    return get('rules.im1Messaging.requiresDetailsRequest')(state);
  },
  [SEND_MESSAGE_SUBJECT_ENABLED](state) {
    return get('rules.im1Messaging.sendMessageSubject')(state);
  },
  [SILVER_INTEGRATION_APPOINTMENTS_ENABLED](state) {
    return get('rules.silverIntegrations.secondaryAppointments.length')(state) > 0;
  },
  [SILVER_INTEGRATION_ENABLED](state) {
    return ({ provider, serviceType }) => getOr([], `rules.silverIntegrations.${serviceType}`, state).includes(provider);
  },
  [SILVER_INTEGRATION_MESSAGES_ENABLED](state) {
    return get('rules.silverIntegrations.messages.length')(state) > 0;
  },
  [UPDATE_STATUS_GP_MESSAGES_ENABLED](state) {
    return get('rules.im1Messaging.canUpdateReadStatus')(state);
  },
};
