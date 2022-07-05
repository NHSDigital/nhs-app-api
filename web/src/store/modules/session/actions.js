import NativeApp from '@/services/native-app';
import semver from 'semver';
import { setCookie, removeCookies } from '@/lib/cookie-manager';
import SessionExpiryModal from '@/components/modal/content/SessionExpiryModal';
import { isAnonymous } from '@/router';
import { getUserAgentNativeVersionNumber } from '@/lib/utils';
import {
  CLEAR,
  INIT,
  LOADED,
  END_VALIDATION_CHECKING,
  LOGOUT,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SHOW_SESSION_EXPIRING,
  HIDE_SESSION_EXPIRING,
  SET_USER_SESSION_REFERENCE,
  HAS_GP_SESSION,
  HAS_ACTIONED_LOGOUT,
} from './mutation-types';
import { LOGOUT_PATH } from '@/router/paths';

export default {
  init:
    ({ commit }) => commit(INIT),
  clear:
    ({ commit }) => commit(CLEAR),
  logout({ commit }) {
    commit(LOGOUT);
    removeCookies({ cookies: this.$cookies, key: ['nhso.session'] });
  },
  hideExpiryMessage:
    ({ commit }) => commit(HIDE_EXPIRY_MESSAGE),
  showExpiryMessage:
    ({ commit }) => commit(SHOW_EXPIRY_MESSAGE),
  async getSession({ commit, dispatch }) {
    const response = await this.app.$http.getV1Session({
      ignoreError: true,
      returnResponse: true,
    });
    if (response !== undefined && response.status === 200) {
      const {
        name,
        odsCode,
        sessionTimeout,
        token,
        nhsNumber,
        dateOfBirth,
        accessToken,
        im1MessagingEnabled,
        userSessionCreateReferenceCode,
        hasGpSession,
        proofLevel,
        patientSessionId,
      } = response.data;

      dispatch('setInfo', {
        user: name,
        durationSeconds: sessionTimeout,
        lastCalledAt: new Date(),
        gpOdsCode: odsCode,
        csrfToken: token,
        nhsNumber,
        dateOfBirth,
        accessToken,
        proofLevel,
        patientSessionId,
      });

      commit(SET_USER_SESSION_REFERENCE, userSessionCreateReferenceCode);
      commit(HAS_GP_SESSION, hasGpSession);

      this.dispatch('practiceSettings/setIm1MessagingEnabled', im1MessagingEnabled);
    }
    commit(LOADED);
    return Promise.resolve();
  },
  updateLastCalledAt({ commit, dispatch }, lastCalledAt = new Date()) {
    dispatch('setInfo', { lastCalledAt });
    commit(HIDE_SESSION_EXPIRING);
  },
  setInfo({ commit, state }, info) {
    const value = !info
      ? undefined
      : { ...state, ...info };

    value.csrfToken = value.csrfToken || value.token;

    value.user = value.user || value.name;

    const cookie = {
      key: 'nhso.session',
      value,
      cookies: this.$cookies,
      secure: this.$env.SECURE_COOKIES,
    };

    // Version check to be removed on ticket NHSO-17380 when Xamarin app is the minimum
    // supported version in production. The cookie created by native code prepends a .
    // to the domain (RCF2109) the web does not resulting in duplicate cookies.
    // This is part of the cause of bug NHSO-16677 which this code helps to resolve
    // by making the domains match.
    const nativeVersion = getUserAgentNativeVersionNumber();
    if (nativeVersion && semver.gte(nativeVersion, this.$env.XAMARIN_INITIAL_RELEASE_VERSION)) {
      cookie.domain = `.${window.location.hostname}`;
    }

    setCookie(cookie);

    commit(SET_INFO, value);
  },
  setGpSession({ commit }, info) {
    commit(HAS_GP_SESSION, info);
  },
  setActionedLogout({ commit }, value) {
    commit(HAS_ACTIONED_LOGOUT, value);
  },
  startValidationChecking({ getters, commit, dispatch, state }) {
    if (!getters.isLoggedIn() || state.validationInterval) return;

    const interval = setInterval(() => {
      dispatch('validate');
    }, 5000);

    commit(START_VALIDATION_CHECKING, interval);
  },
  endValidationChecking: ({ commit, state }) => {
    clearInterval(state.validationInterval);
    commit(END_VALIDATION_CHECKING);
    commit(HIDE_SESSION_EXPIRING);
  },
  validate({ getters, state, commit }, ignoreShowSessionExpiring = false) {
    if (getters.isLoggedIn()) {
      if (getters.isValid()) {
        if (!ignoreShowSessionExpiring && state.showSessionExpiring) {
          return true;
        }

        if (getters.isExpiring(this.$env.SESSION_EXPIRING_WARNING_SECONDS)) {
          commit(SHOW_SESSION_EXPIRING);

          if (window.nativeApp) {
            // ensure any page leave warning still open is dismissed
            this.dispatch('pageLeaveWarning/stayOnPage');

            NativeApp.onSessionExpiring();
          } else {
            this.dispatch('modal/show', { content: SessionExpiryModal });
          }
        }
        return true;
      }

      this.dispatch('auth/logoutWhenExpired');

      this.app.$router.push({ path: `/${LOGOUT_PATH}` });

      return false;
    }

    if (NativeApp.supportsLogout() && !isAnonymous(this.app.$router.currentRoute)) {
      this.dispatch('auth/logoutNativeWhenAlreadyExpired');
    }

    return false;
  },
  extend() {
    return this.app.$http.postV1SessionExtend({ ignoreError: true });
  },
};
