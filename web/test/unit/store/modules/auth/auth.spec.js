/* eslint-disable object-curly-newline */
import { find } from 'lodash/fp';
import actions from '@/store/modules/auth/actions';
import Sources from '@/lib/sources';
import NativeApp from '@/services/native-app';
import { LOGIN_PATH } from '@/router/paths';

jest.mock('@/services/native-app');

describe('actions', () => {
  const name = 'Fozzy Bear';
  const sessionTimeout = 1200;
  const odsCode = 'P123';
  const token = 'sdfdhgmbnrdstgjxjcbv';
  const data = { name, sessionTimeout, odsCode, token };
  const response = { consentGiven: true };
  let commit;
  let state;
  let rootState;

  beforeEach(() => {
    actions.app = {
      $http: {
        postV1Session: jest.fn(() => Promise.resolve({ data })),
        deleteV1Session: jest.fn(() => Promise.resolve()),
        getV1PatientTermsAndConditionsConsent: jest.fn(() => Promise.resolve({ response })),
      },
      $router: {
        go: jest.fn(),
        push: jest.fn(),
      },
      store: {
        state: {
          device: {
            source: Sources.Web,
            referrer: 'test',
          },
        },
      },
      context: {
        redirect: jest.fn(),
      },
    };
    actions.$cookies = {
      remove: jest.fn(),
    };

    commit = jest.fn();
    state = {
      config: {},
      session: {},
    };

    actions.dispatch = jest.fn();
    actions.state = state;

    rootState = {
      device: {
        referrer: 'test',
      },
    };
  });

  afterEach(() => {
    NativeApp.supportsLogout.mockClear();
    NativeApp.logout.mockClear();
  });

  describe('handle auth response', () => {
    it('will set the info from the data received from the server', () => actions
      .handleAuthResponse({ commit, state, rootState }, { code: '123' })
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
    describe('does not support native logout', () => {
      beforeEach(async () => {
        NativeApp.supportsLogout.mockReturnValue(false);
        await actions.logout({ commit });
      });

      it('will dispatch the session/clear event', () => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/clear');
      });

      it('will dispatch the session/endValidationChecking event', () => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/endValidationChecking');
      });

      it('will not call the native logout', () => {
        expect(NativeApp.logout).not.toHaveBeenCalled();
      });

      it('will call router push', () => {
        expect(actions.app.$router.push).toHaveBeenCalledWith({ path: LOGIN_PATH });
      });
    });

    describe('supports native logout', () => {
      beforeEach(async () => {
        NativeApp.supportsLogout.mockReturnValue(true);
        await actions.logout({ commit });
      });

      it('will dispatch the session/clear event', () => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/clear');
      });

      it('will dispatch the session/endValidationChecking event', () => {
        expect(actions.dispatch).toHaveBeenCalledWith('session/endValidationChecking');
      });

      it('will call the native logout', () => {
        expect(NativeApp.logout).toHaveBeenCalled();
      });

      it('will not call router push', () => {
        expect(actions.app.$router.push).not.toHaveBeenCalled();
      });
    });
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
