import onlineConsultations from '@/middleware/onlineConsultations';
import { initialState } from '@/store/modules/onlineConsultations/mutation-types';
import { APPOINTMENT_ADMIN_HELP, APPOINTMENT_GP_ADVICE, APPOINTMENTS, AUTH_RETURN } from '@/lib/routes';
import { IM1_PROVIDER } from '../../../src/store/modules/serviceJourneyRules/mutation-types';

describe('middleware/onlineConsultations', () => {
  let store;

  const callOnlineConsultations = async ({
    routeName, isLoggedIn, isSjrLoaded, adminName, adviceName }) => {
    store = {
      getters: {
        'session/isLoggedIn': () => isLoggedIn,
      },
      dispatch: jest.fn(),
      state: {
        onlineConsultations: initialState(),
        serviceJourneyRules: {
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
              version: 1,
            },
            messaging: false,
            nominatedPharmacy: false,
            notifications: false,
            prescriptions: {
              provider: IM1_PROVIDER,
            },
            hasLinkedAccounts: false,
          },
        },
      },
    };

    store.state.serviceJourneyRules.isLoaded = isSjrLoaded;
    store.state.serviceJourneyRules.rules.cdssAdmin = adminName;
    store.state.serviceJourneyRules.rules.cdssAdvice = adviceName;
    await onlineConsultations({ route: { name: routeName }, store });
  };

  describe('appointments path', () => {
    beforeEach(async () => {
      await callOnlineConsultations({
        routeName: APPOINTMENTS.name,
        isLoggedIn: true,
        isSjrLoaded: true,
        adminName: 'none',
        adviceName: 'none',
      });
    });

    it('will not dispatch `onlineConsultations/setProviderNames`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('admin path', () => {
    beforeEach(async () => {
      await callOnlineConsultations({
        routeName: APPOINTMENT_ADMIN_HELP.name,
        isLoggedIn: true,
        isSjrLoaded: true,
        adminName: 'eConsult',
        adviceName: 'eConsult',
      });
    });

    it('will dispatch `onlineConsultations/setProviderNames`', () => {
      expect(store.dispatch).toBeCalled();
    });
  });

  describe('advice path', () => {
    beforeEach(async () => {
      await callOnlineConsultations({
        routeName: APPOINTMENT_GP_ADVICE.name,
        isLoggedIn: true,
        isSjrLoaded: true,
        adminName: 'eConsult',
        adviceName: 'eConsult',
      });
    });

    it('will dispatch `onlineConsultations/setProviderNames`', () => {
      expect(store.dispatch).toBeCalled();
    });
  });

  describe('advice path sjr not loaded', () => {
    beforeEach(async () => {
      await callOnlineConsultations({
        routeName: APPOINTMENT_GP_ADVICE.name,
        isLoggedIn: true,
        isSjrLoaded: false,
        adminName: 'eConsult',
        adviceName: 'eConsult',
      });
    });

    it('will dispatch `onlineConsultations/setProviderNames`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('auth return path', () => {
    beforeEach(async () => {
      await callOnlineConsultations({
        routeName: AUTH_RETURN.name,
        isLoggedIn: false,
        isSjrLoaded: false,
        adminName: 'none',
        adviceName: 'none',
      });
    });

    it('will dispatch `onlineConsultations/setProviderNames`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });
});
