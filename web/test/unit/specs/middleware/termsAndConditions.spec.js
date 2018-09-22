import { TERMSANDCONDITIONS } from '@/lib/routes';
import { initialState as sessionState } from '@/store/modules/session/mutation-types';
import { initialState as termsAndConditionsState } from '@/store/modules/termsAndConditions/mutation-types';
import getters from '@/store/modules/session/getters';
import termsAndConditions from '@/middleware/termsAndConditions';

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
      route: TERMSANDCONDITIONS,
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

      it('will redirect to "/" if the dispatch says they have been accepted', async () => {
        app.store.dispatch = jest.fn(() => new Promise((resolve) => {
          app.store.state.termsAndConditions.areAccepted = true;
          resolve();
        }));
        await termsAndConditions(app);
        expect(app.redirect).toBeCalledWith('/');
      });
    });

    describe('terms accepted in state', () => {
      beforeEach(async () => {
        app.store.state.termsAndConditions.areAccepted = true;
        await termsAndConditions(app);
      });

      it('will redirect to "/" if the terms have been accepted', async () => {
        expect(app.redirect).toBeCalledWith('/');
      });
    });
  });
});
