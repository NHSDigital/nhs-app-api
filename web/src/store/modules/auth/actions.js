/* eslint-disable global-require */
import NativeCallbacks from '@/services/native-app';
import { LOGIN } from '@/lib/routes';
import { removeCookies } from '@/lib/cookie-manager';
import { AUTH_RESPONSE, LOGOUT, INIT_AUTH, UPDATE_CONFIG } from './mutation-types';

const final = ({ self, commit }) => {
  commit(LOGOUT, true);
  self.dispatch('analytics/init');
  self.dispatch('availableAppointments/init');
  self.dispatch('myAppointments/init');
  self.dispatch('auth/init');
  self.dispatch('device/init');
  self.dispatch('header/init');
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
  self.dispatch('patientPracticeMessaging/init');
  self.dispatch('practiceSettings/init');

  self.app.context.redirect(LOGIN.path);
};

const removeSessionCookies = ({ app }) => removeCookies({
  cookies: app.$cookies,
  key: ['nhso.terms', 'nhso.session', 'NHSO-Session-Id'],
});

const logoutCleanUp = ({ self }) => {
  self.dispatch('session/clear');
  self.dispatch('session/endValidationChecking');
  self.dispatch('errors/disableApiError');
  self.dispatch('navigation/clearPreviousSelectedMenuItem');

  removeSessionCookies(self);
};

export default {
  handleAuthResponse({ commit, state }, code) {
    /**
     * This needs to fire a proxy method
     * as more work needs to be done before logging in
     * for now we will just edit the state object.
     */

    const { codeVerifier, redirectUri: redirectUrl } = state.config || {};
    if (process.server) {
      const { nhsoRequestId } = this.app.context.res.locals;
      const consola = require('consola');
      consola.info(`handleAuthResponse - codeVerifier=${codeVerifier}, redirectUrl=${redirectUrl}, CorrelationId=${nhsoRequestId}`);
    }

    return this.app.$http
      .postV1Session({
        userSession: {
          authCode: code,
          codeVerifier,
          redirectUrl,
        },
      })
      .then((response) => {
        // eslint-disable-next-line object-curly-newline
        const { name, odsCode, sessionTimeout, token, nhsNumber, dateOfBirth, accessToken } =
          (response || {});
        this.dispatch('session/hideExpiryMessage');
        this.dispatch('session/setInfo', {
          name,
          durationSeconds: sessionTimeout,
          gpOdsCode: odsCode,
          token,
          nhsNumber,
          dateOfBirth,
          accessToken,
        });

        commit(AUTH_RESPONSE, response);
        this.dispatch('session/startValidationChecking');

        removeCookies({
          cookies: this.app.$cookies,
          key: 'nhso.auth',
        });
      });
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
    if (process.client) {
      NativeCallbacks.onLogin();
      NativeCallbacks.showHeader();
    }
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
