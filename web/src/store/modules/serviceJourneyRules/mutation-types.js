import { mutationNames } from './constants';

const { IM1_PROVIDER } = mutationNames;

// eslint-disable-next-line import/prefer-default-export
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
    documents: false,
    im1Messaging: {
      isEnabled: false,
      canDeleteMessages: false,
      canUpdateReadStatus: false,
      requiresDetailsRequest: false,
      sendMessageSubject: false,
    },
    medicalRecord: {
      version: '1',
    },
    messaging: false,
    nominatedPharmacy: false,
    notificationPrompt: false,
    notifications: false,
    prescriptions: {
      provider: IM1_PROVIDER,
    },
    silverIntegrations: {
      accountAdmin: [],
      carePlans: [],
      consultations: [],
      consultationsAdmin: [],
      healthTrackers: [],
      libraries: [],
      medicines: [],
      messages: [],
      participation: [],
      recordSharing: [],
      secondaryAppointments: [],
      vaccineRecord: [],
    },
  },
});
