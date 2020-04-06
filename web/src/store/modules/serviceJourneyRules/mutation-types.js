export const CDSS_ADMIN = 'cdssAdmin';
export const CDSS_ADVICE = 'cdssAdvice';
export const GP_AT_HAND = 'gpAtHand';
export const GP_MEDICAL_RECORD = 'gpMedicalRecord';
export const IM1_PROVIDER = 'im1';
export const INFORMATICA = 'informatica';
export const INIT = 'INIT';
export const MESSAGING = 'messaging';
export const NOMINATED_PHARMACY = 'nominatedPharmacy';
export const NOTIFICATIONS = 'notifications';
export const SET_RULES = 'SET_RULES';
export const ONLINE_CONSULTATIONS = 'onlineConsultations';
export const LINKED_ACCOUNT = 'linkedAccount';
export const SILVER_INTEGRATION = 'silverIntegration';
export const DOCUMENTS = 'documents';
export const IM1MESSAGING = 'im1Messaging';
export const DELETE_PATIENT_PRACTICE_MESSAGING = 'deletePatientPracticeMessage';
export const UPDATE_STATUS_PATIENT_PRACTICE_MESSAGING = 'updateStatusPatientPracticeMessage';
export const REQUIRED_DETAILS_CALL_PATIENT_PRACTICE_MESSAGING = 'requiredDetailsCallPatientPracticeMessage';

export const initialState = () => ({
  isLoaded: false,
  rules: {
    appointments: {
      provider: IM1_PROVIDER,
    },
    cdssAdmin: {
      provider: 'none',
    },
    cdssAdvice: {
      provider: 'none',
    },
    medicalRecord: {
      version: '1',
    },
    messaging: false,
    nominatedPharmacy: false,
    notifications: false,
    prescriptions: {
      provider: IM1_PROVIDER,
    },
    silverIntegrations: {
      consultations: [],
      messages: [],
      secondaryAppointments: [],
    },
    documents: false,
    im1Messaging: {
      isEnabled: false,
      canDeleteMessages: false,
      canUpdateReadStatus: false,
      requiresDetailsRequest: false,
    },
  },
});
