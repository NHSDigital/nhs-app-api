import { TERMSANDCONDITIONS, PRESCRIPTIONS } from '@/lib/routes';
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
      route: PRESCRIPTIONS,
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

      it('will not redirect to /terms-and-conditions if the route is an anonymous route', async () => {
        app.route = TERMSANDCONDITIONS;
        await termsAndConditions(app);
        expect(app.redirect).not.toBeCalled();
      });

      it('will redirect to /terms-and-conditions if the route is not an anonymous route', async () => {
        app.route = PRESCRIPTIONS;
        await termsAndConditions(app);
        expect(app.redirect).toBeCalledWith('/terms-and-conditions');
      });

      it('will check the server if the terms and conditions are not accepted on the state', async () => {
        await termsAndConditions(app);
        expect(app.store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will not redirect to "/terms-and-conditions" if the dispatch says they have been accepted', async () => {
        app.store.dispatch = jest.fn(() => new Promise((resolve) => {
          app.store.state.termsAndConditions.areAccepted = true;
          resolve();
        }));
        await termsAndConditions(app);
        expect(app.redirect).not.toBeCalledWith('/terms-and-conditions');
      });

      it('will redirect to "/terms-and-conditions" if the server says they have not been accepted', async () => {
        app.store.dispatch = jest.fn(() => new Promise((resolve) => {
          app.store.state.termsAndConditions.areAccepted = false;
          resolve();
        }));
        await termsAndConditions(app);
        expect(app.redirect).toBeCalledWith('/terms-and-conditions');
      });

      it('will redirect to "/terms-and-conditions" if the terms have not been accepted', async () => {
        await termsAndConditions(app);
        expect(app.redirect).toBeCalledWith('/terms-and-conditions');
      });
    });

    describe('terms accepted in state', () => {
      beforeEach(async () => {
        app.store.state.termsAndConditions.areAccepted = true;
        await termsAndConditions(app);
      });

      it('will not redirect to "/terms-and-conditions" if the terms have been accepted', async () => {
        expect(app.redirect).not.toBeCalledWith('/terms-and-conditions');
      });
    });

    describe('terms accepted in cookie', () => {
      beforeEach(async () => {
        app.store.state.termsAndConditions.areAccepted = false;
        app.$cookies.get.mockReturnValue({ termsAccepted: true });
        await termsAndConditions(app);
      });

      it('will not redirect to "/terms-and-conditions" if the terms have been accepted', () => {
        expect(app.redirect).not.toBeCalledWith('/terms-and-conditions');
      });

      it('will set the flag in the state if it is found in the cookie', () => {
        expect(app.store.dispatch).toBeCalledWith('termsAndConditions/setAcceptance', true);
      });
    });
  });

  describe('is not logged in', () => {
    beforeEach(() => {
      app.store.state.session.csrfToken = undefined;
    });

    it('will not redirect to "/terms-and-conditions" if the terms have not been accepted', async () => {
      await termsAndConditions(app);
      expect(app.redirect).not.toBeCalled();
    });

    it('will not redirect to "/terms-and-conditions" if the terms have been accepted', async () => {
      app.store.state.termsAndConditions.areAccepted = true;
      await termsAndConditions(app);
      expect(app.redirect).not.toBeCalled();
    });
  });
});
