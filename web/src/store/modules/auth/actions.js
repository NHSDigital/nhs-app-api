import NativeApp from '@/services/native-app';
import jwt from 'jwt-decode';
import { LOGOUT_PATH } from '@/router/paths';
import { removeCookies } from '@/lib/cookie-manager';
import get from 'lodash/fp/get';
import { GP_SESSION_ERROR_STATUS, createLocalError } from '@/lib/utils';
import generic from '@/locale/en/generic';
import { INTEGRATION_REFERRER_PARAMETER, SSO_PARAMETER } from '@/router/names';
import { AUTH_RESPONSE, INIT_AUTH, LOGOUT, UPDATE_CONFIG, ADD_GP_SESSION_ERROR } from './mutation-types';

const thirtySeconds = 30000;
const authCookieKey = 'nhso.auth';

const final = ({ self, commit, expired }) => {
  commit(LOGOUT, true);
  self.dispatch('analytics/init');
  self.dispatch('availableAppointments/init');
  self.dispatch('myAppointments/init');
  self.dispatch('auth/init');
  self.dispatch('device/init');
  self.dispatch('http/init');
  self.dispatch('knownServices/init');
  self.dispatch('messaging/init');
  self.dispatch('navigation/init');
  self.dispatch('repeatPrescriptionCourses/init');
  self.dispatch('errors/clearAllApiErrors');
  self.dispatch('session/logout');
  self.dispatch('flashMessage/init');
  self.dispatch('termsAndConditions/init');
  self.dispatch('appVersion/init');
  self.dispatch('organDonation/init');
  self.dispatch('myRecord/init');
  self.dispatch('serviceJourneyRules/init');
  self.dispatch('gpMessages/init');
  self.dispatch('practiceSettings/init');

  if (expired && NativeApp.supportsSessionExpired()) {
    NativeApp.sessionExpired();
    return;
  }

  if (NativeApp.supportsLogout()) {
    NativeApp.logout();
    return;
  }

  self.app.$router.push({ path: LOGOUT_PATH });
};

const removeSessionCookies = self => removeCookies({
  cookies: self.$cookies,
  key: ['nhso.terms', 'nhso.session', 'NHSO-Session-Id', 'NHSO-Session-Expiry'],
});

const logoutCleanUp = ({ self }) => {
  self.dispatch('session/clear');
  self.dispatch('session/endValidationChecking');
  self.dispatch('errors/disableApiError');
  self.dispatch('navigation/clearPreviousSelectedMenuItem');

  removeSessionCookies(self);
};

const getParamValue = (nhsLoginResponse, paramName) => {
  if (nhsLoginResponse.state) {
    const paramsIndex = nhsLoginResponse.state.indexOf('?');
    if (paramsIndex >= 0) {
      const urlSearchParams = new URLSearchParams(nhsLoginResponse.state.substring(paramsIndex));
      return urlSearchParams.get(paramName);
    }
  }
  return null;
};

const getIntegrationReferrer = nhsLoginResponse =>
  getParamValue(nhsLoginResponse, INTEGRATION_REFERRER_PARAMETER);

const attemptedLoginWithSSO = nhsLoginResponse =>
  (getParamValue(nhsLoginResponse, SSO_PARAMETER) === 'true');

const failedSSOLoginMessage = ({ self, nhsLoginResponse }) =>
  self.app.$t('login.authReturn.ssoAttemptedLoginFailure', { referrer: getIntegrationReferrer(nhsLoginResponse) });

const createSessionRequest = (state, rootState, nhsLoginResponse) => {
  const { codeVerifier, redirectUri: redirectUrl } = state.config || {};
  const request = {
    userSession: {
      authCode: get('code', nhsLoginResponse),
      nhsLoginError: get('error', nhsLoginResponse),
      nhsLoginErrorDescription: get('error_description', nhsLoginResponse),
      nhsLoginErrorUri: get('error_uri', nhsLoginResponse),
      codeVerifier,
      redirectUrl,
      integrationReferrer: getIntegrationReferrer(nhsLoginResponse),
    },
    ignoreError: true,
    returnResponse: true,
  };

  if (rootState.device.referrer !== undefined) {
    request.userSession.referrer = rootState.device.referrer;
  }
  return request;
};

const cleanupSession = ({ self }) => {
  self.dispatch('session/startValidationChecking');

  removeCookies({
    cookies: self.$cookies,
    key: authCookieKey,
  });
};

const updateSessionStore = (self, data) => {
  const {
    name,
    odsCode,
    sessionTimeout,
    token,
    nhsNumber,
    dateOfBirth,
    accessToken,
    proofLevel,
    im1MessagingEnabled,
  } = data;
  self.dispatch('session/hideExpiryMessage');
  self.dispatch('practiceSettings/setIm1MessagingEnabled', im1MessagingEnabled);
  self.dispatch('session/setInfo', {
    user: name,
    durationSeconds: sessionTimeout,
    gpOdsCode: odsCode,
    lastCalledAt: new Date(),
    csrfToken: token,
    nhsNumber,
    dateOfBirth,
    accessToken,
    proofLevel,
  });
};

let refreshing;

export default {
  async ensureAccessToken() {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);
    if (decodedToken.exp * 1000 < Date.now() + thirtySeconds) {
      if (refreshing) {
        cookieValue.accessToken = await refreshing;
      } else {
        let resolveRefreshing;
        let rejectRefreshing;

        refreshing = new Promise((resolve, reject) => {
          resolveRefreshing = resolve;
          rejectRefreshing = reject;
        });

        try {
          const { token } = await this.app.$http.postV1PatientAuthorizationAccessTokenRefresh();
          cookieValue.accessToken = token;

          this.dispatch('session/setInfo', { accessToken: token });

          resolveRefreshing(token);
        } catch (e) {
          rejectRefreshing();
          throw e;
        } finally {
          refreshing = null;
        }
      }
    }
    return cookieValue.accessToken;
  },
  async handleAuthResponse({ commit, state, rootState }, nhsLoginResponse) {
    try {
      if (!state.config) {
        this.dispatch('log/onError', generic.errors.noAuthTokenPresent);
        this.dispatch('navigation/goToHomePage');
        return;
      }

      const request = createSessionRequest(state, rootState, nhsLoginResponse);
      const response = await this.app.$http.postV1Session(request);

      updateSessionStore(this, response.data);

      commit(AUTH_RESPONSE, response.data);
    } catch (error) {
      if (attemptedLoginWithSSO(nhsLoginResponse)) {
        await this.dispatch('log/onInfo', failedSSOLoginMessage({ self: this, nhsLoginResponse }));
      }
      this.dispatch('errors/addApiError', error);
    } finally {
      cleanupSession({ self: this });
    }
  },
  async handleGpOnDemandResponse({ commit, state, rootState }, nhsLoginResponse) {
    try {
      commit(ADD_GP_SESSION_ERROR, undefined);
      const request = createSessionRequest(state, rootState, nhsLoginResponse);
      const response = await this.app.$http.putV1SessionGpSessionOnDemand(request);

      updateSessionStore(this, response.data);

      commit(AUTH_RESPONSE, response.data);
    } catch (error) {
      if (error.response.status === GP_SESSION_ERROR_STATUS) {
        commit(ADD_GP_SESSION_ERROR, createLocalError(error));
      } else {
        this.dispatch('errors/addApiError', error);
      }
    } finally {
      cleanupSession({ self: this });
    }
  },
  logoutWhenExpired() {
    this.dispatch('modal/hide');
    this.dispatch('session/showExpiryMessage');
    this.dispatch('auth/logout', true);
  },
  logoutNativeWhenAlreadyExpired() {
    this.dispatch('auth/logout', true);
  },
  logoutNoJs() {
    removeSessionCookies(this);
  },
  logout({ commit }, sessionExpired = false) {
    logoutCleanUp({ self: this });

    return this
      .app
      .$http
      .deleteV1Session()
      .then(() => final({ self: this, commit, expired: sessionExpired }))
      .catch(() => final({ self: this, commit, expired: sessionExpired }));
  },
  init({ commit }) {
    commit(INIT_AUTH);
  },
  nativeLogin() {
    NativeApp.onLogin();
    NativeApp.showHeader();
  },
  updateConfig({ commit }, config) {
    commit(UPDATE_CONFIG, config);
  },
  unauthorised({ commit }) {
    logoutCleanUp({ self: this });
    this.dispatch('session/showExpiryMessage');
    final({ self: this, commit, expired: this.state.session.showExpiryMessage });
  },
};
