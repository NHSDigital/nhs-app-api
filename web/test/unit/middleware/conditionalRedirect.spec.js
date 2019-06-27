import conditionalRedirect from '@/middleware/conditionalRedirect';
import { APPOINTMENTS, APPOINTMENT_INFORMATICA } from '@/lib/routes';

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

  describe('appointments redirect rules', () => {
    describe('sjr informatica disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaEnabled'] = false;
        callConditionalRedirect(APPOINTMENTS);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('sjr informatica enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/informaticaEnabled'] = true;
        callConditionalRedirect(APPOINTMENTS);
      });

      it('will redirect to /appointments/informatica', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENT_INFORMATICA.path);
      });
    });
  });

  describe('appointment informatica redirect rules', () => {
    describe('sjr im1 enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1Enabled'] = true;
        callConditionalRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will redirect to /appointments', () => {
        expect(redirect).toBeCalledWith('301', APPOINTMENTS.path);
      });
    });

    describe('sjr im1 disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/im1Enabled'] = false;
        callConditionalRedirect(APPOINTMENT_INFORMATICA);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });
});
