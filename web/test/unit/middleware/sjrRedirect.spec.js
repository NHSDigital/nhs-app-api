import sjrRedirect from '@/middleware/sjrRedirect';
import {
  ACCOUNT_NOTIFICATIONS,
  APPOINTMENTS,
  APPOINTMENT_GP_AT_HAND,
  APPOINTMENT_INFORMATICA,
  INDEX,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
} from '@/lib/routes';

describe('middleware/sjrRedirect', () => {
  let getters;
  let redirect;
  let store;

  const callSjrRedirect = (route) => {
    sjrRedirect({ redirect, route, store });
  };

  beforeEach(() => {
    getters = [];
    store = {
      getters,
    };
    redirect = jest.fn();
  });

  describe('appointment gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENT_GP_AT_HAND);
      });

      it('will redirect to appointments', () => {
        expect(redirect).toBeCalledWith('302', APPOINTMENTS.path);
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENT_GP_AT_HAND);
      });

      it('will redirect to appointments informatica', () => {
        expect(redirect).toBeCalledWith('302', APPOINTMENT_INFORMATICA.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENT_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('appointments im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENTS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENTS);
      });

      it('will redirect to appointments informatica', () => {
        expect(redirect).toBeCalledWith('302', APPOINTMENT_INFORMATICA.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENTS);
      });

      it('will redirect to appointments gp at hand', () => {
        expect(redirect).toBeCalledWith('302', APPOINTMENT_GP_AT_HAND.path);
      });
    });
  });

  describe('appointment informatica redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will redirect to appointments', () => {
        expect(redirect).toBeCalledWith('302', APPOINTMENTS.path);
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = false;
        callSjrRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callSjrRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will redirect to appointments gp at hand', () => {
        expect(redirect).toBeCalledWith('302', APPOINTMENT_GP_AT_HAND.path);
      });
    });
  });

  describe('my record version 2 redirect rules', () => {
    describe('sjr version 2 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpMedicalRecordV2Enabled'] = true;
        callSjrRedirect(MYRECORD);
      });

      it('will redirect to my record', () => {
        expect(redirect).toBeCalledWith('302', GP_MEDICAL_RECORD.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandGpMedicalRecordV2Enabled'] = true;

        callSjrRedirect(MYRECORD);
      });

      it('will redirect to my record gp at hand', () => {
        expect(redirect).toBeCalledWith('302', GP_MEDICAL_RECORD_GP_AT_HAND.path);
      });
    });

    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1GpMedicalRecordV2Enabled'] = true;

        callSjrRedirect(GP_MEDICAL_RECORD_GP_AT_HAND);
      });

      it('will redirect to my record gp at hand', () => {
        expect(redirect).toBeCalledWith('302', GP_MEDICAL_RECORD.path);
      });
    });
  });

  describe('my record gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MyRecordEnabled'] = true;
        callSjrRedirect(MYRECORD_GP_AT_HAND);
      });

      it('will redirect to my record', () => {
        expect(redirect).toBeCalledWith('302', MYRECORD.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandMyRecordEnabled'] = true;
        callSjrRedirect(MYRECORD_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('my record im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MyRecordEnabled'] = true;
        callSjrRedirect(MYRECORD);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandMyRecordEnabled'] = true;
        callSjrRedirect(MYRECORD);
      });

      it('will redirect to my record gp at hand', () => {
        expect(redirect).toBeCalledWith('302', MYRECORD_GP_AT_HAND.path);
      });
    });
  });

  describe('prescriptions gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS_GP_AT_HAND);
      });

      it('will redirect to prescriptions', () => {
        expect(redirect).toBeCalledWith('302', PRESCRIPTIONS.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('prescriptions im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callSjrRedirect(PRESCRIPTIONS);
      });

      it('will redirect to prescriptions gp at hand', () => {
        expect(redirect).toBeCalledWith('302', PRESCRIPTIONS_GP_AT_HAND.path);
      });
    });
  });

  describe('account notifications rules', () => {
    describe('notifications enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/notificationsEnabled'] = true;
        callSjrRedirect(ACCOUNT_NOTIFICATIONS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('notifications disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/notificationsEnabled'] = false;
        callSjrRedirect(ACCOUNT_NOTIFICATIONS);
      });

      it('will redirect to account', () => {
        expect(redirect).toBeCalledWith('302', INDEX.path);
      });
    });
  });
});
