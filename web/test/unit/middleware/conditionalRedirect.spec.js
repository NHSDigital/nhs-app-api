import conditionalRedirect from '@/middleware/conditionalRedirect';
import {
  APPOINTMENTS,
  APPOINTMENT_GP_AT_HAND,
  APPOINTMENT_INFORMATICA,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
} from '@/lib/routes';

describe('middleware/conditionalRedirect', () => {
  let getters;
  let redirect;
  let store;

  const callConditionalRedirect = (route) => {
    conditionalRedirect({ redirect, route, store });
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
        callConditionalRedirect(APPOINTMENT_GP_AT_HAND);
      });

      it('will redirect to appointments', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENTS.path);
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = true;
        callConditionalRedirect(APPOINTMENT_GP_AT_HAND);
      });

      it('will redirect to appointments informatica', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENT_INFORMATICA.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callConditionalRedirect(APPOINTMENT_GP_AT_HAND);
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
        callConditionalRedirect(APPOINTMENTS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = true;
        callConditionalRedirect(APPOINTMENTS);
      });

      it('will redirect to appointments informatica', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENT_INFORMATICA.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callConditionalRedirect(APPOINTMENTS);
      });

      it('will redirect to appointments gp at hand', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENT_GP_AT_HAND.path);
      });
    });
  });

  describe('appointment informatica redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = true;
        callConditionalRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will redirect to appointments', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENTS.path);
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = false;
        callConditionalRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandAppointmentsEnabled'] = true;
        callConditionalRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will redirect to appointments gp at hand', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENT_GP_AT_HAND.path);
      });
    });
  });

  describe('my record gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MyRecordEnabled'] = true;
        callConditionalRedirect(MYRECORD_GP_AT_HAND);
      });

      it('will redirect to my record', () => {
        expect(redirect).toBeCalledWith('301', MYRECORD.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandMyRecordEnabled'] = true;
        callConditionalRedirect(MYRECORD_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('my record im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1MyRecordRedirect'] = true;
        callConditionalRedirect(MYRECORD);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandMyRecordEnabled'] = true;
        callConditionalRedirect(MYRECORD);
      });

      it('will redirect to my record gp at hand', () => {
        expect(redirect).toBeCalledWith('301', MYRECORD_GP_AT_HAND.path);
      });
    });
  });

  describe('prescriptions gp at hand redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = true;
        callConditionalRedirect(PRESCRIPTIONS_GP_AT_HAND);
      });

      it('will redirect to prescriptions', () => {
        expect(redirect).toBeCalledWith('301', PRESCRIPTIONS.path);
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callConditionalRedirect(PRESCRIPTIONS_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('prescriptions im1 redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsRedirect'] = true;
        callConditionalRedirect(PRESCRIPTIONS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callConditionalRedirect(PRESCRIPTIONS);
      });

      it('will redirect to prescriptions gp at hand', () => {
        expect(redirect).toBeCalledWith('301', PRESCRIPTIONS_GP_AT_HAND.path);
      });
    });
  });
});
