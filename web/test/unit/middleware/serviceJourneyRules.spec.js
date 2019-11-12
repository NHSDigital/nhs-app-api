import serviceJourneyRules from '@/middleware/serviceJourneyRules';
import { initialState } from '@/store/modules/serviceJourneyRules/mutation-types';
import { AUTH_RETURN, APPOINTMENTS } from '@/lib/routes';

describe('middleware/serviceJourneyRules', () => {
  let store;

  const callServiceJourneyRules
   = async ({ routeName, isLoggedIn, isLoaded, linkedAccountsConfigLoaded, actingAsUser }) => {
     store = {
       getters: {
         'session/isLoggedIn': () => isLoggedIn,
       },
       dispatch: jest.fn(),
       state: {
         serviceJourneyRules: initialState(),
         linkedAccounts: initialState(),
       },
     };

     store.state.serviceJourneyRules.isLoaded = isLoaded;
     store.state.linkedAccounts.actingAsUser = actingAsUser;
     store.state.linkedAccounts.config = {};
     store.state.linkedAccounts.config.hasLoaded = linkedAccountsConfigLoaded;
     await serviceJourneyRules({ route: { name: routeName }, store });
   };

  describe('anonymous path', () => {
    beforeEach(async () => {
      await callServiceJourneyRules({
        routeName: AUTH_RETURN.name,
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
      });
    });

    it('will not dispatch `serviceJourneyRules/load`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('not logged in', () => {
    beforeEach(async () => {
      await callServiceJourneyRules({
        routeName: APPOINTMENTS.name,
        isLoggedIn: false,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
      });
    });

    it('will not dispatch `serviceJourneyRules/load`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('already loaded', () => {
    beforeEach(async () => {
      await callServiceJourneyRules({
        routeName: APPOINTMENTS.name,
        isLoggedIn: true,
        isLoaded: true,
        linkedAccountsConfigLoaded: true,
      });
    });

    it('will not dispatch `serviceJourneyRules/load`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('logged in and not anonymous or loaded', () => {
    beforeEach(async () => {
      await callServiceJourneyRules({
        routeName: APPOINTMENTS.name,
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
      });
    });

    it('will dispatch `serviceJourneyRules/load`', () => {
      expect(store.dispatch).toHaveBeenCalledWith('serviceJourneyRules/load');
    });
  });

  describe('logged in and not anonymous or loaded and acting as user', () => {
    beforeEach(async () => {
      await callServiceJourneyRules({
        routeName: APPOINTMENTS.name,
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: true,
        actingAsUser: {},
      });
    });

    it('will dispatch `serviceJourneyRules/loadLinkedAccount`', () => {
      expect(store.dispatch).toHaveBeenCalledWith('serviceJourneyRules/loadLinkedAccount');
    });
  });
});
