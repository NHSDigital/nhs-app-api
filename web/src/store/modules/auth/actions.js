import NativeApp from '@/services/native-app';
import jwt from 'jwt-decode';
import { LOGIN_PATH } from '@/router/paths';
import { removeCookies, setCookie } from '@/lib/cookie-manager';
import { AUTH_RESPONSE, INIT_AUTH, LOGOUT, UPDATE_CONFIG } from './mutation-types';

const thirtySeconds = 30000;

const final = ({ self, commit }) => {
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
  self.dispatch('session/setInfo');
  self.dispatch('flashMessage/init');
  self.dispatch('termsAndConditions/init');
  self.dispatch('appVersion/init');
  self.dispatch('organDonation/init');
  self.dispatch('myRecord/init');
  self.dispatch('serviceJourneyRules/init');
  self.dispatch('gpMessages/init');
  self.dispatch('practiceSettings/init');

  if (NativeApp.supportsLogout()) {
    NativeApp.logout();
  } else {
    self.app.$router.push({ path: LOGIN_PATH });
  }
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

const createSessionRequest = (state, rootState, code) => {
  const { codeVerifier, redirectUri: redirectUrl } = state.config || {};
  const request = {
    userSession: {
      authCode: code,
      codeVerifier,
      redirectUrl,
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
    key: 'nhso.auth',
  });
};

const updateSessionStore = (self, data, updateType) => {
  const {
    name,
    odsCode,
    sessionTimeout,
    token,
    nhsNumber,
    dateOfBirth,
    accessToken,
    proofLevel,
  } = data;
  self.dispatch('session/hideExpiryMessage');
  self.dispatch(updateType, {
    name,
    durationSeconds: sessionTimeout,
    gpOdsCode: odsCode,
    token,
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

          setCookie({
            key: 'nhso.session',
            value: cookieValue,
            cookies: this.$cookies,
            secure: this.$env.SECURE_COOKIES,
          });

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
  async handleAuthResponse({ commit, state, rootState }, code) {
    try {
      const request = createSessionRequest(state, rootState, code);
      const response = await this.app.$http.postV1Session(request);

      updateSessionStore(this, response.data, 'session/setInfo');

      commit(AUTH_RESPONSE, response.data);
    } catch (error) {
      this.dispatch('errors/addApiError', error);
    } finally {
      cleanupSession({ self: this });
    }
  },
  async handleGpOnDemandResponse({ commit, state, rootState }, code) {
    try {
      const request = createSessionRequest(state, rootState, code);
      const response = await this.app.$http.putV1SessionGpSessionOnDemand(request);

      updateSessionStore(this, response.data, 'session/updateInfo');

      commit(AUTH_RESPONSE, response.data);
    } catch (error) {
      this.dispatch('errors/addApiError', error);
    } finally {
      cleanupSession({ self: this });
    }
  },
  logoutWhenExpired() {
    this.dispatch('modal/hide');
    this.dispatch('session/showExpiryMessage');
    this.dispatch('auth/logout', { expired: true });
  },
  logoutNoJs() {
    removeSessionCookies(this);
  },
  logout({ commit }, { expired } = {}) {
    logoutCleanUp({ self: this });

    return this
      .app
      .$http
      .deleteV1Session()
      .then(() => final({ self: this, commit, expired }))
      .catch(() => final({ self: this, commit, expired }));
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
