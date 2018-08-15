import actions from '../../../src/store/modules/auth/actions';

describe('actions', () => {
  const sessionTimeout = 1200;
  const odsCode = 'P123';
  const token = 'sdfdhgmbnrdstgjxjcbv';
  let commit;
  let state;

  beforeEach(() => {
    actions.app = {
      $http: {
        postV1Session: jest.fn(() => Promise.resolve({ sessionTimeout, odsCode, token })),
        deleteV1Session: jest.fn(() => Promise.resolve()),
      },
      router: {
        go: jest.fn(),
        push: jest.fn(),
      },
    };

    commit = jest.fn();
    state = {
      config: {},
      session: {},
    };

    actions.dispatch = jest.fn();
    actions.state = state;
  });

  describe('handle auth response', () => {
    it('will set the duration seconds from the received session timeout', () => actions
      .handleAuthResponse({ commit, state }, { code: '123' })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/setDurationSeconds', sessionTimeout);
      }));
    it('will set the gp ods code from the received code', () => actions
      .handleAuthResponse({ commit, state }, { code: '123' })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/setGpOdsCode', odsCode);
      }));
    it('will set the csrfToken from the received token in the response.', () => actions
      .handleAuthResponse({ commit, state }, { code: '200' })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/setCsrfToken', token);
      }));
  });

  describe('logout', () => {
    it('will dispatch the session/clear event', () => actions
      .logout({ commit })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/clear');
      }));

    it('will dispatch the session/endValidationChecking event', () => actions
      .logout({ commit })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/endValidationChecking');
      }));
    it('will clear the csrf token by dispatching the session/setCsrfToken event', () => actions
      .logout({ commit })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/setCsrfToken', '');
      }));
  });

  describe('logoutWhenExpired', () => {
    it('will dispatch the session/showExpiryMessage and auth/logout event', () => {
      actions
        .logoutWhenExpired();

      expect(actions.dispatch).toHaveBeenCalledWith('session/showExpiryMessage');
      expect(actions.dispatch).toHaveBeenCalledWith('auth/logout');
    });
  });
});
