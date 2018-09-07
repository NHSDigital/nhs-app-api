import { AUTH_RESPONSE, LOGOUT, INIT_AUTH, UPDATE_CONFIG } from './mutation-types';

const MAX_TRIES = 10;

const final = ({ self, commit, expired }) => {
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

  if (expired) {
    // When the session is expired, a `push` must be used to ensure the state and,
    // by implication, the `showExpiryMessage` property is preserved.
    self.app.router.push('login');
  } else {
    // When logout occurs through the button, `go` is used to reduce flickering.  This makes
    // a server request which clears the state.
    self.app.router.go('/login');
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
        const { name, odsCode, sessionTimeout, token, nhsNumber, dateOfBirth } = (response || {});
        this.dispatch('session/hideExpiryMessage');

        // TODO: Fix
        // This will not work using the new authentication mechanisms.
        // <<<<<<< HEAD
        //         this.dispatch('session/setCsrfToken', response.token);
        //         this.app.$http
        //           .getV1PatientTermsAndConditionsConsent({})
        //           .then((data) => {
        //             if (data.response) {
        //               if (data.response.consentGiven === true) {
        //                 commit(AUTH_RESPONSE, response);
        //                 this.dispatch('session/startValidationChecking');
        //                 this.app.router.push({
        //                   name: 'index',
        //                 });
        //               } else {
        //                 this.app.router.push({
        //                   name: 'terms-and-conditions',
        //                   params: { authResponse: response },
        //                 });
        //               }
        //             } else {
        //               this.app.router.push({
        //                 name: 'terms-and-conditions',
        //                 params: { authResponse: response },
        //               });
        //             }
        //           });
        //       });
        //   },
        //   goHandleAuthResponse({ commit }, message) {
        //     return this.app.$http
        //       .postV1PatientTermsAndConditionsConsent(message.a)
        //       .then(() => {
        //         commit(AUTH_RESPONSE, message.b);
        // =======
        this.dispatch('session/setInfo', {
          name,
          durationSeconds: sessionTimeout,
          gpOdsCode: odsCode,
          token,
          nhsNumber,
          dateOfBirth,
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
  logout({ commit }, { expired } = {}) {
    this.dispatch('session/clear');
    this.dispatch('session/endValidationChecking');
    this.dispatch('errors/disableApiError');

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
    const login = () => {
      if (window.nativeApp) {
        window.nativeApp.onLogin();
        return true;
      }

      return false;
    };

    if (process.server) return;

    if (!login()) {
      let tries = 0;
      const interval = setInterval(() => {
        tries += 1;
        if (login() && tries <= MAX_TRIES) {
          tries = 0;
          clearInterval(interval);
        }
      }, 500);
    }
  },
  updateConfig({ commit }, config) {
    commit(UPDATE_CONFIG, config);
  },
  unauthorised({ commit }) {
    final({ self: this, commit });
  },
};
