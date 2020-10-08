import getters from '@/store/modules/session/getters';
import termsAndConditions from '@/middleware/termsAndConditions';
import { TERMS_AND_CONDITIONS } from '@/router/routes/login';
import { LOGOUT } from '@/router/routes/logout';
import { APPOINTMENTS } from '@/router/routes/appointments';
import {
  TERMSANDCONDITIONS_NAME,
  APPOINTMENTS_NAME,
  REDIRECT_PARAMETER,
  INTERSTITIAL_REDIRECTOR_NAME,
  NOTIFICATIONS_NAME,
} from '@/router/names';
import { initialState as sessionState } from '@/store/modules/session/mutation-types';
import { initialState as termsAndConditionsState } from '@/store/modules/termsAndConditions/mutation-types';
import * as dependency from '@/lib/utils';

const { isLoggedIn } = getters;

describe('middleware/termsAndConditions', () => {
  let context;
  dependency.createRouteByNameObject = jest.fn(x => ({ name: x.name, query: x.query }));

  beforeEach(() => {
    const state = {
      session: sessionState(),
      termsAndConditions: termsAndConditionsState(),
    };

    context = {
      next: jest.fn(),
      store: {
        state,
        commit: jest.fn(),
        dispatch: jest.fn(),
        getters: {
          'session/isLoggedIn': isLoggedIn(state),
        },
        $cookies: {
          get: jest.fn(),
          set: jest.fn(),
        },
      },
      to: {
        ...TERMS_AND_CONDITIONS,
        query: {},
      },
    };
  });

  describe('is logged in', () => {
    beforeEach(() => {
      context.store.state.session.csrfToken = 'token';
    });

    describe('terms not accepted', () => {
      beforeEach(() => {
        context.store.state.termsAndConditions.areAccepted = false;
      });

      it('will check the server if the terms and conditions are not accepted on the state', async () => {
        context.to = TERMS_AND_CONDITIONS;
        await termsAndConditions(context);
        expect(context.store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will allow logout regardless of acceptance of the terms', async () => {
        context.to = LOGOUT;
        await termsAndConditions(context);
        expect(context.next).not.toBeCalledWith(expect.anything);
        expect(context.next).toBeCalled();
      });

      describe('will redirect to', () => {
        beforeEach(() => {
          context.to = APPOINTMENTS;
          context.to.query = {};
        });

        it('terms and conditions when not accepted', async () => {
          await termsAndConditions(context);
          expect(context.next).toBeCalledWith({
            name: TERMSANDCONDITIONS_NAME,
            query: {},
          });
        });

        it('terms and conditions with redirect query param when not accepted', async () => {
          context.to.query = { [REDIRECT_PARAMETER]: LOGOUT.name };
          await termsAndConditions(context);
          expect(context.next).toBeCalledWith({
            name: TERMSANDCONDITIONS_NAME,
            query: { [REDIRECT_PARAMETER]: LOGOUT.name },
          });
        });
      });

      describe('dispatch says terms have been accepted', () => {
        beforeEach(async () => {
          context.store.dispatch = jest.fn(() => new Promise((resolve) => {
            context.store.state.termsAndConditions.areAccepted = true;
            resolve();
          }));
        });

        it('will redirect to NOTIFICATIONS', async () => {
          await termsAndConditions(context);
          expect(context.next).toHaveBeenCalledWith({
            name: NOTIFICATIONS_NAME,
            query: {},
          });
        });

        it('will redirect to query param target', async () => {
          context.to.query = { [REDIRECT_PARAMETER]: APPOINTMENTS_NAME };
          await termsAndConditions(context);
          expect(context.next).toBeCalledWith({
            name: APPOINTMENTS_NAME,
            query: {},
          });
        });

        it('will redirect to query parameter target and pass source parameter', async () => {
          context.to.query = { [REDIRECT_PARAMETER]: APPOINTMENTS_NAME, source: 'web' };
          await termsAndConditions(context);
          expect(context.next).toBeCalledWith({
            name: APPOINTMENTS_NAME,
            query: { source: 'web' },
          });
        });
      });
    });

    describe('terms accepted in state', () => {
      describe('no redirect in query string', () => {
        beforeEach(async () => {
          context.store.state.termsAndConditions.areAccepted = true;
          await termsAndConditions(context);
        });

        it('will redirect to NOTIFICATIONS', async () => {
          expect(context.next).toBeCalledWith({
            name: NOTIFICATIONS_NAME,
            query: {},
          });
        });
      });

      describe('internal route redirect in query string', () => {
        beforeEach(async () => {
          context.store.state.termsAndConditions.areAccepted = true;
          context.to.query = { [REDIRECT_PARAMETER]: APPOINTMENTS_NAME };
          await termsAndConditions(context);
        });

        it('will redirect to target of redirect_to parameter', async () => {
          expect(context.next).toBeCalledWith({
            name: APPOINTMENTS_NAME,
            query: {},
          });
        });
      });

      describe('external route redirect in query string', () => {
        beforeEach(async () => {
          context.store.state.termsAndConditions.areAccepted = true;
          context.to.query = { [REDIRECT_PARAMETER]: 'somethingelse', source: 'web' };
          await termsAndConditions(context);
        });

        it('will redirect to redirector page with redirect_to parameter', async () => {
          expect(context.next).toBeCalledWith({
            name: INTERSTITIAL_REDIRECTOR_NAME,
            query: context.to.query,
          });
        });
      });
    });
  });
});
