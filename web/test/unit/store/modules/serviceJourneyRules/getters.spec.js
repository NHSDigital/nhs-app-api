import each from 'jest-each';
import { mutationNames } from '@/store/modules/serviceJourneyRules/constants';
import getters from '@/store/modules/serviceJourneyRules/getters';
import { initialState } from '@/store/modules/serviceJourneyRules/mutation-types';

const {
  GP_AT_HAND,
  IM1_PROVIDER,
  INFORMATICA,
  LINKED_ACCOUNT,
} = mutationNames;

describe('getters', () => {
  let currentState;

  beforeEach(() => {
    currentState = initialState();
  });

  describe('gpAtHandAppointmentsEnabled', () => {
    const { gpAtHandAppointmentsEnabled } = getters;

    beforeEach(() => {
      currentState = {
        rules: {
          appointments: {
            provider: 'gpAtHand',
          },
        },
      };
    });

    it('will be true if the appointments provider is gp at hand', () => {
      currentState.rules.appointments.provider = GP_AT_HAND;
      expect(gpAtHandAppointmentsEnabled(currentState)).toBe(true);
    });

    it('will be false if the appointments provider is informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(gpAtHandAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be false if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(gpAtHandAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be false if the appointments provider is linkedAccount', () => {
      currentState.rules.appointments.provider = LINKED_ACCOUNT;
      expect(gpAtHandAppointmentsEnabled(currentState)).toBe(false);
    });
  });

  describe('im1AppointmentsEnabled', () => {
    const { im1AppointmentsEnabled } = getters;

    it('will be false if the appointments provider is gp at hand', () => {
      currentState.rules.appointments.provider = GP_AT_HAND;
      expect(im1AppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be true if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(im1AppointmentsEnabled(currentState)).toBe(true);
    });

    it('will be false if the appointments provider is informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(im1AppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be false if the appointments provider is linkedAccount', () => {
      currentState.rules.appointments.provider = LINKED_ACCOUNT;
      expect(im1AppointmentsEnabled(currentState)).toBe(false);
    });
  });

  describe('informaticaAppointmentsEnabled', () => {
    const { informaticaAppointmentsEnabled } = getters;

    it('will be false if the appointments provider is gp at hand', () => {
      currentState.rules.appointments.provider = GP_AT_HAND;
      expect(informaticaAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be true if the appointments provider is Informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(informaticaAppointmentsEnabled(currentState)).toBe(true);
    });

    it('will be false if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(informaticaAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be false if the appointments provider is linkedAccount', () => {
      currentState.rules.appointments.provider = LINKED_ACCOUNT;
      expect(informaticaAppointmentsEnabled(currentState)).toBe(false);
    });
  });

  describe('linkedAccountAppointmentsEnabled', () => {
    const { linkedAccountAppointmentsEnabled } = getters;

    it('will be false if the appointments provider is gp at hand', () => {
      currentState.rules.appointments.provider = GP_AT_HAND;
      expect(linkedAccountAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be false if the appointments provider is Informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(linkedAccountAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be false if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(linkedAccountAppointmentsEnabled(currentState)).toBe(false);
    });

    it('will be true if the appointments provider is linkedAccount', () => {
      currentState.rules.appointments.provider = LINKED_ACCOUNT;
      expect(linkedAccountAppointmentsEnabled(currentState)).toBe(true);
    });
  });

  describe('gpAtHandMyRecordEnabled', () => {
    const { gpAtHandMyRecordEnabled } = getters;

    beforeEach(() => {
      currentState = {
        rules: {
          medicalRecord: {
            provider: 'gpAtHand',
          },
        },
      };
    });

    it('will be true if the medicalRecord provider is gp at hand', () => {
      currentState.rules.medicalRecord.provider = GP_AT_HAND;
      expect(gpAtHandMyRecordEnabled(currentState)).toBe(true);
    });

    it('will be false if the medicalRecord provider is im1', () => {
      currentState.rules.medicalRecord.provider = IM1_PROVIDER;
      expect(gpAtHandMyRecordEnabled(currentState)).toBe(false);
    });
  });

  describe('im1MyRecordEnabled', () => {
    const { im1MyRecordEnabled } = getters;

    beforeEach(() => {
      currentState = {
        rules: {
          medicalRecord: {
            provider: 'im1',
          },
        },
      };
    });

    it('will be false if the medicalRecord provider is gp at hand', () => {
      currentState.rules.medicalRecord.provider = GP_AT_HAND;
      expect(im1MyRecordEnabled(currentState)).toBe(false);
    });

    it('will be true if the medicalRecord provider is im1', () => {
      currentState.rules.medicalRecord.provider = IM1_PROVIDER;
      expect(im1MyRecordEnabled(currentState)).toBe(true);
    });
  });

  describe('gpAtHandPrescriptionsEnabled', () => {
    const { gpAtHandPrescriptionsEnabled } = getters;

    beforeEach(() => {
      currentState = {
        rules: {
          prescriptions: {
            provider: 'gpAtHand',
          },
        },
      };
    });

    it('will be true if the prescriptions provider is gp at hand', () => {
      currentState.rules.prescriptions.provider = GP_AT_HAND;
      expect(gpAtHandPrescriptionsEnabled(currentState)).toBe(true);
    });

    it('will be false if the prescriptions provider is im1', () => {
      currentState.rules.prescriptions.provider = IM1_PROVIDER;
      expect(gpAtHandPrescriptionsEnabled(currentState)).toBe(false);
    });
  });

  describe('im1PrescriptionsEnabled', () => {
    const { im1PrescriptionsEnabled } = getters;
    beforeEach(() => {
      currentState = {
        rules: {
          prescriptions: {
            provider: 'im1',
          },
        },
      };
    });

    it('will be false if the prescriptions provider is gp at hand', () => {
      currentState.rules.prescriptions.provider = GP_AT_HAND;
      expect(im1PrescriptionsEnabled(currentState)).toBe(false);
    });

    it('will be true if the prescriptions provider is im1', () => {
      currentState.rules.prescriptions.provider = IM1_PROVIDER;
      expect(im1PrescriptionsEnabled(currentState)).toBe(true);
    });
  });

  describe('messagingEnabled', () => {
    const { messagingEnabled } = getters;

    it('will be true if messaging is true', () => {
      currentState.rules.messaging = true;
      expect(messagingEnabled(currentState)).toBe(true);
    });

    it('will be false if messaging is false', () => {
      currentState.rules.messaging = false;
      expect(messagingEnabled(currentState)).toBe(false);
    });
  });

  describe('nominatedPharmacyEnabled', () => {
    const { nominatedPharmacyEnabled } = getters;

    it('will be true if nominated pharmacy is true', () => {
      currentState.rules.nominatedPharmacy = true;
      expect(nominatedPharmacyEnabled(currentState)).toBe(true);
    });

    it('will be false if nominated pharmacy is false', () => {
      currentState.rules.nominatedPharmacy = false;
      expect(nominatedPharmacyEnabled(currentState)).toBe(false);
    });
  });

  describe('notificationsEnabled', () => {
    const { notificationsEnabled } = getters;

    it('will be true if notifications is true', () => {
      currentState.rules.notifications = true;
      expect(notificationsEnabled(currentState)).toBe(true);
    });

    it('will be false if notifications is false', () => {
      currentState.rules.notifications = false;
      expect(notificationsEnabled(currentState)).toBe(false);
    });
  });

  describe('documentsEnabled', () => {
    const { documentsEnabled } = getters;

    it('will be true if documents is true', () => {
      currentState.rules.documents = true;
      expect(documentsEnabled(currentState)).toBe(true);
    });

    it('will be false if documents is false', () => {
      currentState.rules.documents = false;
      expect(documentsEnabled(currentState)).toBe(false);
    });
  });

  describe('im1MessagingIsEnabled', () => {
    const { im1MessagingEnabled } = getters;

    it('will be true if im1Messaging is enabled is true', () => {
      currentState.rules.im1Messaging.isEnabled = true;
      expect(im1MessagingEnabled(currentState)).toBe(true);
    });

    it('will be false if im1Messaging is enabled is false', () => {
      currentState.rules.im1Messaging.isEnabled = false;
      expect(im1MessagingEnabled(currentState)).toBe(false);
    });
  });

  describe('im1MessagingCanDeleteMessagesEnabled', () => {
    const { deleteGpMessagesEnabled } = getters;

    it('will be true if im1Messaging can delete messages is true', () => {
      currentState.rules.im1Messaging.canDeleteMessages = true;
      expect(deleteGpMessagesEnabled(currentState))
        .toBe(true);
    });

    it('will be false if im1Messaging can delete messages  is false', () => {
      currentState.rules.im1Messaging.canDeleteMessages = false;
      expect(deleteGpMessagesEnabled(currentState))
        .toBe(false);
    });
  });

  describe('Silver Integration Appointments', () => {
    const { silverIntegrationAppointmentsEnabled } = getters;

    it('will be true if silverIntegrations.secondaryAppointments is populated', () => {
      currentState.rules.silverIntegrations.secondaryAppointments = ['pkb', 'pkbCie'];
      expect(silverIntegrationAppointmentsEnabled(currentState))
        .toBe(true);
    });

    it('will be false if silverIntegrations.secondaryAppointments is empty', () => {
      currentState.rules.silverIntegrations.secondaryAppointments = [];
      expect(silverIntegrationAppointmentsEnabled(currentState))
        .toBe(false);
    });
  });

  each([
    [['pkb', 'pkbCie'], ['pkb', 'pkbCie'], true],
    [[], ['pkb', 'pkbCie'], true],
    [['pkb', 'pkbCie'], [], true],
    [['pkbCie'], ['pkbCie'], true],
    [['pkb', 'pkbCie'], [], true],
    [[], [], false],
  ]).describe('Silver Integration My Record Hub', (
    providerCarePlans, providerHealthTrackers, expectedResult,
  ) => {
    const { myRecordHubEnabled } = getters;

    it(`will be ${expectedResult} if carePlans is ${providerCarePlans} or 
    HealthTrackers is ${providerHealthTrackers}`, () => {
      currentState.rules.silverIntegrations.carePlans = providerCarePlans;
      currentState.rules.silverIntegrations.healthTrackers = providerHealthTrackers;
      expect(myRecordHubEnabled(currentState))
        .toBe(expectedResult);
    });
  });
});
