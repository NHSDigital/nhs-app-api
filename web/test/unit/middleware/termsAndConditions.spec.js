import getters from '@/store/modules/session/getters';
import termsAndConditions from '@/middleware/termsAndConditions';
import { APPOINTMENTS, INTERSTITIAL_REDIRECTOR, LOGOUT, REDIRECT_PARAMETER, TERMSANDCONDITIONS } from '@/lib/routes';
import { initialState as sessionState } from '@/store/modules/session/mutation-types';
import { initialState as termsAndConditionsState } from '@/store/modules/termsAndConditions/mutation-types';

const { isLoggedIn } = getters;

describe('middleware/termsAndConditions', () => {
  let app;

  beforeEach(() => {
    const state = {
      session: sessionState(),
      termsAndConditions: termsAndConditionsState(),
    };

    app = {
      $cookies: {
        get: jest.fn(),
        set: jest.fn(),
      },
      redirect: jest.fn(),
      store: {
        state,
        commit: jest.fn(),
        dispatch: jest.fn(),
        getters: {
          'session/isLoggedIn': isLoggedIn(state),
        },
      },
      route: {
        ...TERMSANDCONDITIONS,
        query: {},
      },
    };
    app.store.app = app;
  });

  describe('is logged in', () => {
    beforeEach(() => {
      app.store.state.session.csrfToken = 'token';
    });

    describe('terms not accepted', () => {
      beforeEach(() => {
        app.store.state.termsAndConditions.areAccepted = false;
      });

      it('will check the server if the terms and conditions are not accepted on the state', async () => {
        app.route = TERMSANDCONDITIONS;
        await termsAndConditions(app);
        expect(app.store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will allow logout regardless of acceptance of the terms', async () => {
        app.route = LOGOUT;
        await termsAndConditions(app);
        expect(app.redirect).not.toBeCalled();
      });

      describe('will redirect to', () => {
        beforeEach(() => {
          app.route = { APPOINTMENTS,
            query: {},
          };
        });

        it('terms and conditions when not accepted', async () => {
          await termsAndConditions(app);
          expect(app.redirect).toBeCalledWith(TERMSANDCONDITIONS.path, {});
        });

        it('terms and conditions with redirect query param when not accepted', async () => {
          app.route.query = { [REDIRECT_PARAMETER]: LOGOUT.name };
          await termsAndConditions(app);
          expect(app.redirect).toBeCalledWith(TERMSANDCONDITIONS.path,
            { [REDIRECT_PARAMETER]: LOGOUT.name });
        });
      });

      describe('dispatch says terms have been accepted', () => {
        beforeEach(async () => {
          app.store.dispatch = jest.fn(() => new Promise((resolve) => {
            app.store.state.termsAndConditions.areAccepted = true;
            resolve();
          }));
        });

        it('will redirect to "/"', async () => {
          await termsAndConditions(app);
          expect(app.redirect).toHaveBeenCalledWith('/', {});
        });

        it('will redirect to query param target', async () => {
          app.route.query = { [REDIRECT_PARAMETER]: APPOINTMENTS.name };
          await termsAndConditions(app);
          expect(app.redirect).toBeCalledWith(APPOINTMENTS.path);
        });

        it('will redirect to query parameter target and pass source parameter', async () => {
          app.route.query = { [REDIRECT_PARAMETER]: APPOINTMENTS.name, source: 'web' };
          await termsAndConditions(app);
          expect(app.redirect).toBeCalledWith(APPOINTMENTS.path);
        });
      });
    });

    describe('terms accepted in state', () => {
      describe('no redirect in query string', () => {
        beforeEach(async () => {
          app.store.state.termsAndConditions.areAccepted = true;
          await termsAndConditions(app);
        });

        it('will redirect to "/"', async () => {
          expect(app.redirect).toBeCalledWith('/', {});
        });
      });

      describe('internal route redirect in query string', () => {
        beforeEach(async () => {
          app.store.state.termsAndConditions.areAccepted = true;
          app.route.query = { [REDIRECT_PARAMETER]: APPOINTMENTS.name };
          await termsAndConditions(app);
        });

        it('will redirect to target of redirect_to parameter', async () => {
          expect(app.redirect).toBeCalledWith(APPOINTMENTS.path);
        });
      });

      describe('external route redirect in query string', () => {
        beforeEach(async () => {
          app.store.state.termsAndConditions.areAccepted = true;
          app.route.query = { [REDIRECT_PARAMETER]: 'somethingelse', source: 'web' };
          await termsAndConditions(app);
        });

        it('will redirect to redirector page with redirect_to parameter', async () => {
          expect(app.redirect).toBeCalledWith(INTERSTITIAL_REDIRECTOR.path, app.route.query);
        });
      });
    });
  });
});
