import NativeCallbacks from '@/services/native-app';
import Sources from '@/lib/sources';
import { LOGIN } from '@/lib/routes';
import { AUTH_RESPONSE, LOGOUT, INIT_AUTH, UPDATE_CONFIG } from './mutation-types';

const MAX_TRIES = 10;

const final = ({ self, commit }) => {
  const sourceValue = self.app.store.state.device.source;

  commit(LOGOUT, true);
  self.dispatch('analytics/init');
  self.dispatch('availableAppointments/init');
  self.dispatch('myAppointments/init');
  self.dispatch('auth/init');
  self.dispatch('device/init');
  self.dispatch('header/init');
  self.dispatch('http/init');
  self.dispatch('navigation/init');
  self.dispatch('prescriptions/init');
  self.dispatch('repeatPrescriptionCourses/init');
  self.dispatch('errors/clearAllApiErrors');
  self.dispatch('session/setInfo');
  self.dispatch('flashMessage/init');
  self.dispatch('termsAndConditions/init');
  self.dispatch('appVersion/init');

  if (sourceValue === Sources.Web) {
    self.app.context.redirect(LOGIN.path);
  } else {
    self.app.context.redirect(`${LOGIN.path}?source=${sourceValue}`);
  }
};

export default {
  handleAuthResponse({ commit, state }, code) {
    /**
     * This needs to fire a proxy method
     * as more work needs to be done before logging in
     * for now we will just edit the state object.
     */

    const { codeVerifier, redirectUri: redirectUrl } = state.config || {};
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
        this.app.$cookies.remove('nhso.auth');
      });
  },

  logoutWhenExpired() {
    this.dispatch('session/showExpiryMessage');
    this.dispatch('auth/logout', { expired: true });
  },
  logoutNoJs() {
    this.app.$cookies.removeAll();
  },
  logout({ commit }, { expired } = {}) {
    this.dispatch('session/clear');
    this.dispatch('session/endValidationChecking');
    this.dispatch('errors/disableApiError');
    this.dispatch('navigation/clearPreviousSelectedMenuItem');

    this.app.$cookies.remove('nhso.session');

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
    if (!process.server && !NativeCallbacks.onLogin()) {
      let attempts = 0;
      const interval = setInterval(() => {
        attempts += 1;
        if (NativeCallbacks.onLogin() || attempts >= MAX_TRIES) {
          clearInterval(interval);
        }
      }, 500);
    }
  },
  updateConfig({ commit }, config) {
    commit(UPDATE_CONFIG, config);
  },
  unauthorised({ commit }) {
    final({ self: this, commit, expired: this.state.session.showExpiryMessage });
  },
};
