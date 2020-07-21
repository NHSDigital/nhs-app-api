import NativeCallbacks from '@/services/native-app';
import { setCookie } from '@/lib/cookie-manager';
import {
  CLEAR,
  INIT,
  LOADED,
  END_VALIDATION_CHECKING,
  HIDE_EXPIRY_MESSAGE,
  SET_INFO,
  SET_LAST_CALLED_AT,
  SHOW_EXPIRY_MESSAGE,
  START_VALIDATION_CHECKING,
  SHOW_SESSION_EXPIRING,
  HIDE_SESSION_EXPIRING,
  SET_USER_SESSION_REFERENCE,
} from './mutation-types';
import SessionExpiryModal from '@/components/modal/content/SessionExpiryModal';

export default {
  init:
    ({ commit }) => commit(INIT),
  clear:
    ({ commit }) => commit(CLEAR),
  hideExpiryMessage:
    ({ commit }) => commit(HIDE_EXPIRY_MESSAGE),
  showExpiryMessage:
    ({ commit }) => commit(SHOW_EXPIRY_MESSAGE),
  async getSession({ commit }) {
    const response = await this.app.$http.getV1Session({
      ignoreError: true,
      returnResponse: true,
    });
    if (response.status === 200) {
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
        proofLevel,
      } = response.data;

      commit(SET_INFO, {
        name,
        durationSeconds: sessionTimeout,
        gpOdsCode: odsCode,
        token,
        nhsNumber,
        dateOfBirth,
        accessToken,
        proofLevel,
      });

      commit(SET_USER_SESSION_REFERENCE, userSessionCreateReferenceCode);

      this.dispatch('practiceSettings/setIm1MessagingEnabled', im1MessagingEnabled);
    }
    commit(LOADED);
    return Promise.resolve();
  },
  updateLastCalledAt({ commit }, lastCalledAt = new Date()) {
    if (process.client || !this.app.context.res.locals.LastCalledAtUpdated) {
      if (process.server) {
        this.app.context.res.locals.LastCalledAtUpdated = true;
      }

      const session = this.app.$cookies.get('nhso.session');

      if (session) {
        session.lastCalledAt = lastCalledAt;
        setCookie({
          key: 'nhso.session',
          value: session,
          cookies: this.app.$cookies,
          options: {
            secure: this.app.$env.SECURE_COOKIES,
          },
        });
      }
    }

    commit(SET_LAST_CALLED_AT, lastCalledAt);
    commit(HIDE_SESSION_EXPIRING);
  },
  setInfo({ commit, state }, info) {
    const value = !info
      ? undefined
      : ({
        name: info.name || state.user,
        durationSeconds: info.durationSeconds,
        gpOdsCode: info.gpOdsCode,
        token: info.token,
        lastCalledAt: info.lastCalledAt || new Date(),
        nhsNumber: info.nhsNumber,
        dateOfBirth: info.dateOfBirth,
        accessToken: info.accessToken,
        proofLevel: info.proofLevel,
      });

    setCookie({
      key: 'nhso.session',
      value,
      cookies: this.app.$cookies,
      options: {
        secure: this.app.$env.SECURE_COOKIES,
      },
    });

    commit(SET_INFO, info);
  },
  startValidationChecking({ getters, commit, dispatch, state }) {
    if (process.server || !getters.isLoggedIn() || state.validationInterval) return;

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
  validate({ getters, state, commit }) {
    if (getters.isLoggedIn()) {
      if (getters.isValid()) {
        if (process.client && !state.showSessionExpiring
            && getters.isExpiring(this.app.$env.SESSION_EXPIRING_WARNING_SECONDS)) {
          commit(SHOW_SESSION_EXPIRING);

          if (window.nativeApp) {
            // ensure any page leave warning still open is dismissed
            this.dispatch('pageLeaveWarning/stayOnPage');

            NativeCallbacks.onSessionExpiring();
          } else {
            this.dispatch('modal/show', { content: SessionExpiryModal });
          }
        }
        return true;
      }
      this.dispatch('auth/logoutWhenExpired');
    }
    return false;
  },
  extend({ dispatch }) {
    return this.app.$http.postV1SessionExtend()
      .catch(() => dispatch('auth/logoutWhenExpired'));
  },
};
