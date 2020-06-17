import NativeCallbacks from '@/services/native-app';
import actions from '@/store/modules/auth/actions';
import each from 'jest-each';
import jwt from 'jwt-decode';
import mockdate from 'mockdate';
import sources from '@/lib/sources';
import { AUTH_RESPONSE, LOGOUT, UPDATE_CONFIG } from '@/store/modules/auth/mutation-types';
import { mockCookies } from '../../../helpers';

jest.mock('@/services/native-app');

describe('actions', () => {
  const mockAccessToken = 'eyJzdWIiOiIzYzkwZDJkNy01NDc0LTRhY2QtODcyMi1jN2MzMzI4ODE0MjEiLCJhdWQiOiJuaHMtb25saW5lIiwia2lkIjoiNmQ2OWI3MjI2YTA5NDNiZGNlMzI4ZjFjMmYyZjFiNDMzYjI4NjlmMCIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU4ODMyNDQ1NSwiaWF0IjoxNTg4MzI0MTU1LCJhbGciOiJSUzUxMiIsImp0aSI6Ijk3NGZiNTMyLTFlYzktNDM2ZC05OTliLTY3Y2EzODI3ODg0ZiJ9.eyJzdWIiOiIzYzkwZDJkNy01NDc0LTRhY2QtODcyMi1jN2MzMzI4ODE0MjEiLCJuaHNfbnVtYmVyIjoiNTc4NTQ0NTg3NSIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI1Nzg1NDQ1ODc1IiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU4ODMyNDE1MSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIHByb2ZpbGVfZXh0ZW5kZWQiLCJ2b3QiOiJQOS5DcC5DZCIsImV4cCI6MTU4ODMyNDQ1NSwiaWF0IjoxNTg4MzI0MTU1LCJyZWFzb25fZm9yX3JlcXVlc3QiOiJwYXRpZW50YWNjZXNzIiwianRpIjoiOTc0ZmI1MzItMWVjOS00MzZkLTk5OWItNjdjYTM4Mjc4ODRmIn0.ZR5WUyRjAcW0KVfMZQOI10ccBfZ3XUR6OcChBNP-xSHl9D_RJAJ6ujjzOiuabnNnbZvWUmHphSGdwHZTodlYb0E3OQSxqLwgsaI1fOgbHNfEIwVWp_U_zsJqD78XZUECO9ZFOSolRBp3tHk0wT5C9PI3yFgWVogL0Tw92lqay_NL1V_V_YoaV2Vp-Wfa3XIXp-Ivsowrd6_iydeopE0iUcKxzaLR6ZnqIKhkSsUZZW7ICAxaJDLgVy1hGcbo6a37CQxqNcHzMaLA_9rQhVl51N8igzGThlh3I21iImMCe75j95GmnC_VAvcfCZVYHDOA0fngKgI-towzRGKPxEBF8w';
  const refreshedToken = 'new access token';
  const name = 'Montel';
  const odsCode = 'A12345';
  const sessionTimeout = 1200;
  const token = 'sdfdhgmbnrdstgjxjcbv';
  const postSessionResponse = {
    data: {
      odsCode,
      name,
      sessionTimeout,
      token,
    },
  };
  let commit;
  let state;

  const finalAsserts = () => {
    it('will commit the value `true` to `LOGOUT`', () => {
      expect(commit).toBeCalledWith(LOGOUT, true);
    });

    each([
      'analytics/init',
      'appVersion/init',
      'auth/init',
      'availableAppointments/init',
      'device/init',
      'errors/clearAllApiErrors',
      'flashMessage/init',
      'http/init',
      'messaging/init',
      'myAppointments/init',
      'myRecord/init',
      'navigation/init',
      'organDonation/init',
      'repeatPrescriptionCourses/init',
      'serviceJourneyRules/init',
      'session/setInfo',
      'termsAndConditions/init',
      'gpMessages/init',
      'practiceSettings/init',
    ]).it('will dispatch the `%s` event', (action) => {
      expect(actions.dispatch).toHaveBeenCalledWith(action);
    });
  };

  const removeSessionCookiesAsserts = () => {
    it('will remove the `nhso.session` cookie', () => {
      expect(actions.$cookies.remove).toHaveBeenCalledWith('nhso.session');
    });

    it('will remove the `nhso.terms` cookie', () => {
      expect(actions.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });

    it('will remove the `NHSO-Session-Id` cookie', () => {
      expect(actions.$cookies.remove).toHaveBeenCalledWith('nhso.terms');
    });
  };

  const logoutCleanUpAsserts = () => {
    each([
      'session/clear',
      'session/endValidationChecking',
      'errors/disableApiError',
      'navigation/clearPreviousSelectedMenuItem',
    ]).it('will dispatch the `%s` event', (action) => {
      expect(actions.dispatch).toHaveBeenCalledWith(action);
    });

    removeSessionCookiesAsserts();
  };

  beforeEach(() => {
    actions.app = {
      $http: {
        postV1PatientAuthorizationAccessTokenRefresh: jest.fn(() =>
          Promise.resolve({ token: refreshedToken })),
        postV1Session: jest.fn(() => Promise.resolve(postSessionResponse)),
        deleteV1Session: jest.fn(() => Promise.resolve()),
      },
      $router: {
        go: jest.fn(),
        push: jest.fn(),
      },
      store: {
        state: {
          device: { source: sources.Web },
        },
      },
      context: {
        redirect: jest.fn(),
      },
    };
    actions.$env = {
      SECURE_COOKIES: true,
    };
    actions.$cookies = {
      ...mockCookies(),
      get: (cookieName) => {
        switch (cookieName) {
          case 'nhso.session':
            return {
              accessToken: mockAccessToken,
            };
          default:
            return undefined;
        }
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
    beforeEach(async () => {
      await actions.handleAuthResponse({ commit, state }, '123');
    });

    it('will set the session info from the received session timeout and the response', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/setInfo', {
        name,
        durationSeconds: sessionTimeout,
        token,
        gpOdsCode: odsCode,
      });
    });

    it('will hide start validation checking', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/hideExpiryMessage');
    });

    it('will hide the expiry message', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('session/startValidationChecking');
    });

    it('will remove the "nhso.auth" cookie', () => {
      expect(actions.$cookies.remove).toHaveBeenCalledWith('nhso.auth');
    });

    it('will commit the AUTH_RESPONSE', () => {
      expect(commit).toHaveBeenCalledWith(AUTH_RESPONSE, postSessionResponse.data);
    });
  });

  describe('ensureAccessToken', () => {
    let expiryDate;

    beforeEach(() => {
      const decodedJwt = jwt(mockAccessToken);
      expiryDate = new Date(decodedJwt.exp * 1000);
    });

    describe('Token will expire in 30 seconds', () => {
      let result;

      beforeEach(async () => {
        mockdate.set(expiryDate.setSeconds(expiryDate.getSeconds() - 29));
        result = await actions.ensureAccessToken();
      });

      it('will refresh the token', () => {
        expect(actions.app.$http.postV1PatientAuthorizationAccessTokenRefresh).toBeCalled();
      });

      it('will set `nhso.session` with refreshed token', () => {
        expect(actions.$cookies.set).toBeCalledWith(
          'nhso.session',
          { accessToken: refreshedToken },
          { path: '/', secure: actions.$env.SECURE_COOKIES },
        );
      });

      it('will return refreshed token', () => {
        expect(result).toBe(refreshedToken);
      });
    });

    describe('Token will not expire in 30 seconds', () => {
      let result;

      beforeEach(async () => {
        mockdate.set(expiryDate.setSeconds(expiryDate.getSeconds() - 30));
        result = await actions.ensureAccessToken();
      });

      it('will not refresh the token', () => {
        expect(actions.app.$http.postV1PatientAuthorizationAccessTokenRefresh).not.toBeCalled();
      });

      it('will not set cookie', () => {
        expect(actions.$cookies.set).not.toBeCalled();
      });

      it('will return the existing token', () => {
        expect(result).toBe(mockAccessToken);
      });
    });
  });

  describe('logout', () => {
    beforeEach(async () => {
      await actions.logout({ commit });
    });

    logoutCleanUpAsserts();

    removeSessionCookiesAsserts();

    finalAsserts();
  });

  describe('logoutNoJs', () => {
    beforeEach(async () => {
      await actions.logoutNoJs();
    });

    removeSessionCookiesAsserts();
  });

  describe('logoutWhenExpired', () => {
    beforeEach(() => {
      actions.logoutWhenExpired();
    });

    each([
      'session/showExpiryMessage',
      'modal/hide',

    ]).it('will dispatch the `%s` event', (action) => {
      expect(actions.dispatch).toHaveBeenCalledWith(action);
    });

    it('will dispatch the `auth/logout` event', () => {
      expect(actions.dispatch).toHaveBeenCalledWith('auth/logout', { expired: true });
    });
  });

  describe('updateConfig', () => {
    it('will call commit with the sent value', () => {
      const newConfigValue = { test: 'value' };

      actions.updateConfig({ commit }, newConfigValue);

      expect(commit).toBeCalledWith(UPDATE_CONFIG, newConfigValue);
    });
  });

  describe('nativeLogin', () => {
    it('will fire the native onLogin callback', () => {
      actions.nativeLogin();
      expect(NativeCallbacks.onLogin).toHaveBeenCalled();
    });

    it('will fire the show headers callback', () => {
      actions.nativeLogin();
      expect(NativeCallbacks.showHeader).toHaveBeenCalled();
    });
  });

  describe('unauthorised', () => {
    beforeEach(() => {
      actions.unauthorised({ commit });
    });

    logoutCleanUpAsserts();

    finalAsserts();
  });
});
