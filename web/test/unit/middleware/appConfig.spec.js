import appConfig from '@/middleware/appConfig';
import { initialState } from '@/store/modules/serviceJourneyRules/mutation-types';
import { APPOINTMENTS } from '@/router/routes/appointments';
import { AUTH_RETURN } from '@/router/routes/login';

describe('middleware/appConfig', () => {
  let store;

  const callServiceJourneyRules
   = async ({ to, isLoggedIn, isLoaded, linkedAccountsConfigLoaded,
     actingAsUser, hasLinkedAccounts = false,
     mainPatientId = null }) => {
     store = {
       getters: {
         'session/isLoggedIn': () => isLoggedIn,
         'linkedAccounts/mainPatientId': mainPatientId,
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
     store.state.linkedAccounts.config.hasLinkedAccounts = hasLinkedAccounts;
     store.state.linkedAccounts.items = [{
       id: '1234',
     }];
     await appConfig({ to, store, next: jest.fn() });
   };

  describe('anonymous path', () => {
    beforeEach(async () => {
      await callServiceJourneyRules({
        to: AUTH_RETURN,
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
        to: APPOINTMENTS,
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
        to: APPOINTMENTS,
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
        to: APPOINTMENTS,
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
        to: APPOINTMENTS,
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

  describe('logged in and not anonymous, linked account config not loaded, and has linked accounts', () => {
    it('will dispatch `linkedAccounts/initialiseConfig`', async () => {
      await callServiceJourneyRules({
        to: { APPOINTMENTS, params: { patientId: 'differentId' } },
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
        actingAsUser: {},
        hasLinkedAccounts: true,
      });
      expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/initialiseConfig');
    });

    it('will dispatch `linkedAccounts/switchToMainUserProfile` when param id matches mainPatientId', async () => {
      await callServiceJourneyRules({
        to: { APPOINTMENTS, params: { patientId: 'mainPatientId' } },
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
        actingAsUser: {},
        hasLinkedAccounts: true,
        mainPatientId: 'mainPatientId',
      });
      expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchToMainUserProfile');
    });

    it('will dispatch `linkedAccounts/switchToMainUserProfile` when param id is null', async () => {
      await callServiceJourneyRules({
        to: { APPOINTMENTS, params: { patientId: null } },
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
        actingAsUser: {},
        hasLinkedAccounts: true,
        mainPatientId: 'mainPatientId',
      });
      expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchToMainUserProfile');
    });

    it('will dispatch `linkedAccounts/switchProfile` when param id matches one in the linked accounts', async () => {
      await callServiceJourneyRules({
        to: { APPOINTMENTS, params: { patientId: '1234' } },
        isLoggedIn: true,
        isLoaded: false,
        linkedAccountsConfigLoaded: false,
        actingAsUser: {},
        hasLinkedAccounts: true,
        mainPatientId: 'mainPatientId',
      });
      expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchProfile', store.state.linkedAccounts.items[0]);
      expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/select', store.state.linkedAccounts.items[0]);
      expect(store.dispatch).toHaveBeenCalledWith('linkedAccounts/loadAccountAccessSummary', store.state.linkedAccounts.items[0].id);
    });
  });
});
