import conditionalRedirect from '@/middleware/conditionalRedirect';
import { APPOINTMENTS, APPOINTMENT_INFORMATICA, PRESCRIPTIONS, PRESCRIPTION_GP_AT_HAND } from '@/lib/routes';

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

  describe('appointments im1 redirect rules', () => {
    describe('sjr informatica disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaAppointmentsEnabled'] = false;
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

    describe('sjr im1 disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1AppointmentsEnabled'] = false;
        callConditionalRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });

  describe('prescriptions im1 redirect rules', () => {
    describe('sjr gp at hand for prescriptions disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = false;
        callConditionalRedirect(PRESCRIPTIONS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr gp at hand for prescriptions enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/gpAtHandPrescriptionsEnabled'] = true;
        callConditionalRedirect(PRESCRIPTIONS);
      });

      it('will redirect to prescriptions gp at hand', () => {
        expect(redirect).toBeCalledWith('301', PRESCRIPTION_GP_AT_HAND.path);
      });
    });
  });

  describe('prescriptions gp at hand redirect rules', () => {
    describe('sjr im1 prescriptions enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = true;
        callConditionalRedirect(PRESCRIPTION_GP_AT_HAND);
      });

      it('will redirect to prescriptions', () => {
        expect(redirect).toBeCalledWith('301', PRESCRIPTIONS.path);
      });
    });

    describe('sjr im1 prescriptions disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1PrescriptionsEnabled'] = false;
        callConditionalRedirect(PRESCRIPTION_GP_AT_HAND);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });
});
