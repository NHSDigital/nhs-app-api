import getters from '@/store/modules/serviceJourneyRules/getters';
import { initialState, GP_AT_HAND, IM1_PROVIDER, INFORMATICA } from '@/store/modules/serviceJourneyRules/mutation-types';

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
});
