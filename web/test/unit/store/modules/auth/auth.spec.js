/* eslint-disable object-curly-newline */
import { find } from 'lodash/fp';
import actions from '../../../../../src/store/modules/auth/actions';
import Sources from '../../../../../src/lib/sources';

describe('actions', () => {
  const name = 'Fozzy Bear';
  const sessionTimeout = 1200;
  const odsCode = 'P123';
  const token = 'sdfdhgmbnrdstgjxjcbv';
  const data = { name, sessionTimeout, odsCode, token };
  const response = { consentGiven: true };
  let commit;
  let state;

  beforeEach(() => {
    actions.app = {
      $http: {
        postV1Session: jest.fn(() => Promise.resolve({ data })),
        deleteV1Session: jest.fn(() => Promise.resolve()),
        getV1PatientTermsAndConditionsConsent: jest.fn(() => Promise.resolve({ response })),
      },
      router: {
        go: jest.fn(),
        push: jest.fn(),
      },
      $cookies: {
        remove: jest.fn(),
      },
      store: {
        state: {
          device: { source: Sources.Web },
        },
      },
      context: {
        redirect: jest.fn(),
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
    it('will set the info from the data received from the server', () => actions
      .handleAuthResponse({ commit, state }, { code: '123' })
      .then(() => {
        const call = find(x => x[0] === 'session/setInfo')(actions.dispatch.mock.calls);
        expect(call).not.toBeUndefined();
        expect(call.length).toEqual(2);

        const {
          name: actualName,
          durationSeconds: actualTimeout,
          gpOdsCode: actualOdsCode,
          token: actualToken,
        } = call[1];

        expect(actualName).toEqual(name);
        expect(actualTimeout).toEqual(sessionTimeout);
        expect(actualOdsCode).toEqual(odsCode);
        expect(actualToken).toEqual(token);
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
  });

  describe('logoutWhenExpired', () => {
    it('will dispatch the session/showExpiryMessage and auth/logout event', () => {
      actions
        .logoutWhenExpired();

      expect(actions.dispatch).toHaveBeenCalledWith('modal/hide');
      expect(actions.dispatch).toHaveBeenCalledWith('session/showExpiryMessage');
      expect(actions.dispatch).toHaveBeenCalledWith('auth/logout', { expired: true });
    });
  });
});
