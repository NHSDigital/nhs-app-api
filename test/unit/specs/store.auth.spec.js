import actions from '../../../src/store/modules/auth/actions';

describe('actions', () => {
  const sessionTimeout = 1200;
  let commit;
  let state;

  beforeEach(() => {
    actions.app = {
      $http: {
        postV1Session: jest.fn(() => Promise.resolve({ sessionTimeout })),
        deleteV1Session: jest.fn(() => Promise.resolve()),
      },
      router: [],
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
  });

  describe('logout', () => {
    it('will dispatch the session/clear event', () => actions
      .logout({ commit })
      .then(() => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/clear');
      }));
  });

  describe('logout', () => {
    it('will dispatch the session/showExpiryMessage if the lastUpdatedAt is set', () => {
      state.session.lastUpdatedAt = new Date();
      actions
        .logout({ commit });

      expect(actions.dispatch).toHaveBeenCalledWith('session/showExpiryMessage');
    });

    it('will dispatch the session/clear event', () => {
      actions
        .logout({ commit });

      expect(actions.dispatch).toHaveBeenCalledWith('session/clear');
    });
  });
});
