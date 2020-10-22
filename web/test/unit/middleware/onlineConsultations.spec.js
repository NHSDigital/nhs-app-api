import onlineConsultations from '@/middleware/onlineConsultations';
import { initialState } from '@/store/modules/onlineConsultations/mutation-types';
import { mutationNames } from '@/store/modules/serviceJourneyRules/constants';
import {
  APPOINTMENTS_NAME,
  APPOINTMENT_ADMIN_HELP_NAME,
  GP_ADVICE_NAME,
  AUTH_RETURN_NAME,
} from '@/router/names';

const { IM1_PROVIDER } = mutationNames;

describe('middleware/onlineConsultations', () => {
  let store;
  const next = jest.fn();

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
              version: '1',
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
    await onlineConsultations({ to: { name: routeName }, store, next });
  };

  describe('appointments path', () => {
    beforeEach(async () => {
      await callOnlineConsultations({
        routeName: APPOINTMENTS_NAME,
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
        routeName: APPOINTMENT_ADMIN_HELP_NAME,
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
        routeName: GP_ADVICE_NAME,
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
        routeName: GP_ADVICE_NAME,
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
        routeName: AUTH_RETURN_NAME,
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
