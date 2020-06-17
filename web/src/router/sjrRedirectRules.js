import {
  HEALTH_RECORDS_PATH,
  GP_APPOINTMENTS_PATH,
  APPOINTMENT_INFORMATICA_PATH,
  APPOINTMENT_GP_AT_HAND_PATH,
  GP_MEDICAL_RECORD_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTIONS_GP_AT_HAND_PATH,
  INDEX_PATH,
  APPOINTMENTS_PATH,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
  GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
  GP_MESSAGES_PATH,
} from '@/router/paths';

import {
  HEALTH_RECORDS_NAME,
  GP_APPOINTMENTS_NAME,
  APPOINTMENT_INFORMATICA_NAME,
  APPOINTMENT_GP_AT_HAND_NAME,
  GP_MEDICAL_RECORD_NAME,
  PRESCRIPTIONS_NAME,
  PRESCRIPTIONS_GP_AT_HAND_NAME,
  INDEX_NAME,
  APPOINTMENTS_NAME,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  GP_MEDICAL_RECORD_GP_AT_HAND_NAME,
  GP_MESSAGES_NAME,
} from '@/router/names';

export default {
  adminHelpDisabledRedirect: {
    journey_disabled: 'cdssAdmin',
    url: '/appointments',
  },
  deleteMessageRedirect: {
    journey_disabled: 'deleteGpMessages',
    url: GP_MESSAGES_PATH,
    name: GP_MESSAGES_NAME,
  },
  documentsDisabledRedirect: {
    journey_disabled: 'documents',
    url: GP_MEDICAL_RECORD_PATH,
    name: GP_MEDICAL_RECORD_NAME,
  },
  gpAdviceDisabledRedirect: {
    journey_disabled: 'cdssAdvice',
    url: GP_APPOINTMENTS_PATH,
    name: GP_APPOINTMENTS_NAME,
  },
  gpAtHandAppointmentRedirect: {
    journey: 'gpAtHandAppointments',
    url: APPOINTMENT_GP_AT_HAND_PATH,
    name: APPOINTMENT_GP_AT_HAND_NAME,
  },
  gpAtHandMyRecordRedirect: {
    journey: 'gpAtHandMyRecord',
    url: GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
    name: GP_MEDICAL_RECORD_GP_AT_HAND_NAME,
  },
  gpAtHandMedicalRecordRedirectV2: {
    journey: 'gpAtHandGpMedicalRecordV2',
    url: GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
    name: GP_MEDICAL_RECORD_GP_AT_HAND_NAME,
  },
  gpAtHandPrescriptionsRedirect: {
    journey: 'gpAtHandPrescriptions',
    url: PRESCRIPTIONS_GP_AT_HAND_PATH,
    name: PRESCRIPTIONS_GP_AT_HAND_NAME,
  },
  im1AppointmentRedirect: {
    journey: 'im1Appointments',
    url: GP_APPOINTMENTS_PATH,
    name: GP_APPOINTMENTS_NAME,
  },
  im1GpMedicalRecordRedirectV2: {
    journey: 'im1GpMedicalRecordV2',
    url: HEALTH_RECORDS_PATH,
    name: HEALTH_RECORDS_NAME,
  },
  im1MessagingDisabledRedirect: {
    journey_disabled: 'im1Messaging',
    url: INDEX_PATH,
    name: INDEX_NAME,
  },
  im1PrescriptionsRedirect: {
    journey: 'im1Prescriptions',
    url: PRESCRIPTIONS_PATH,
    name: PRESCRIPTIONS_NAME,
  },
  informaticaAppointmentRedirect: {
    journey: 'informaticaAppointments',
    url: APPOINTMENT_INFORMATICA_PATH,
    name: APPOINTMENT_INFORMATICA_NAME,
  },
  messagingDisabledRedirect: {
    journey_disabled: 'messaging',
    url: INDEX_PATH,
    name: INDEX_NAME,
  },
  linkedAccountAppointmentRedirect: {
    journey: 'linkedAccountAppointments',
    url: LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
    name: LINKED_PROFILES_SHUTTER_APPOINTMENTS_NAME,
  },
  silverIntegrationsSecondaryAppointmentsDisabledRedirect: {
    journey_disabled: 'silverIntegrationAppointments',
    url: APPOINTMENTS_PATH,
    name: APPOINTMENTS_NAME,
  },
  silverIntegrationsHealthRecordHubCarePlansEnabledRedirect: {
    journey: 'myRecordHub',
    url: HEALTH_RECORDS_PATH,
    name: HEALTH_RECORDS_NAME,
  },
};
