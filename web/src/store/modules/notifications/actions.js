import get from 'lodash/fp/get';
import NativeApp from '@/services/native-app';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import jwt from 'jwt-decode';
import {
  NOTIFICATIONS_NAME,
  AUTH_RETURN_NAME,
  TERMSANDCONDITIONS_NAME,
  USER_RESEARCH_NAME,
} from '@/router/names';
import { NOTIFICATIONS_GENERIC_FAILURE_PATH } from '@/router/paths';
import { setCookie } from '@/lib/cookie-manager';
import {
  SET_NOTIFICATION_COOKIE_EXISTS,
  SET_REGISTRATION, SET_WAITING,
  TOGGLE_UPDATED,
} from './mutation-types';

const load = 'load';
const toggle = 'toggle';
const authorisationStatus = {
  authorised: 'authorised',
  denied: 'denied',
  notDetermined: 'notDetermined',
};

const addApiError = ({ dispatch }, errorCode, message) => dispatch('errors/addApiError', {
  message,
  response: {
    status: 500,
    data: {
      errorCode,
    },
  },
});

let resolveTask = () => { };

export default {
  authorised({ commit, dispatch, state }, deviceResponse) {
    let deviceResponseParam = deviceResponse;

    if (typeof deviceResponseParam !== 'object') {
      deviceResponseParam = JSON.parse(deviceResponse);
    }

    const { trigger, devicePns, deviceType } = deviceResponseParam;

    if (trigger === toggle) {
      const registering = !state.registered;
      const promise = registering
        ? this.app.$http.postV1ApiUsersMeDevices({ addDeviceRequest: { devicePns, deviceType } })
        : this.app.$http.deleteV1ApiUsersMeDevices({ devicePns });

      return promise
        .then(() => {
          commit(SET_REGISTRATION, registering);

          if (this.app.$router.currentRoute.name === NOTIFICATIONS_NAME) {
            dispatch('logMetrics', {
              screenShown: true,
              notificationsRegistered: registering,
              didErrorAttemptingToUpdateStatus: false,
            });
          }
        })
        .catch((error) => {
          if (this.app.$router.currentRoute.name === NOTIFICATIONS_NAME) {
            dispatch('logMetrics', {
              screenShown: true,
              notificationsRegistered: state.registered,
              didErrorAttemptingToUpdateStatus: true,
            });
            this.app.$router.push({ path: NOTIFICATIONS_GENERIC_FAILURE_PATH });
          } else {
            if (get('response.status')(error) === 404) {
              addApiError(this, 10003, error.message);
            }
            throw new Error(error.message);
          }
        })
        .finally(() => {
          commit(SET_WAITING, false);
          resolveTask(authorisationStatus.authorised);
        });
    }

    return this.app.$http.getV1ApiUsersMeDevices({ devicePns })
      .then(() => commit(SET_REGISTRATION, true))
      .catch(() => commit(SET_REGISTRATION, false))
      .finally(() => {
        resolveTask(authorisationStatus.authorised);
      });
  },
  load() {
    const loading = new Promise((resolve) => {
      resolveTask = resolve;
    });

    NativeApp.getNotificationsStatus();
    return loading;
  },
  async logMetrics({ rootState }, params) {
    const { screenShown, notificationsRegistered, didErrorAttemptingToUpdateStatus } = params;
    const platform = rootState.device.source;

    try {
      await this.app.$http.postV1ApiUsersMeDevicesPromptMetrics({
        notificationsPromptData: {
          screenShown,
          notificationsRegistered,
          platform,
          didErrorAttemptingToUpdateStatus,
        },
      });
    } catch {
      // do nothing as this is just logging
    }
  },
  settingsStatus({ commit }, status) {
    switch (status) {
      case authorisationStatus.authorised:
        NativeApp.requestPnsToken(load);
        break;
      case authorisationStatus.denied:
        commit(SET_REGISTRATION, false);

        if (get('name')(this.app.$router.history.pending) === NOTIFICATIONS_NAME) {
          this.app.$router.push({ path: NOTIFICATIONS_GENERIC_FAILURE_PATH });
        } else {
          addApiError(this, 10001);
        }

        resolveTask(status);
        break;
      default:
        commit(SET_REGISTRATION, false);
        resolveTask(status);
    }
  },
  retryToggle({ dispatch }) {
    this.dispatch('errors/clearAllApiErrors');
    EventBus.$emit(UPDATE_HEADER, 'navigation.pages.headers.notifications');
    EventBus.$emit(UPDATE_TITLE, 'navigation.pages.headers.notifications');
    return dispatch('toggle');
  },
  toggle({ commit }) {
    commit(SET_WAITING, true);
    commit(TOGGLE_UPDATED, true);

    const toggling = new Promise((resolve) => {
      resolveTask = resolve;
    });

    NativeApp.requestPnsToken(toggle);
    return toggling;
  },
  unauthorised({ commit, dispatch }) {
    commit(SET_WAITING, false);
    switch (this.app.$router.currentRoute.name) {
      case NOTIFICATIONS_NAME:
      case USER_RESEARCH_NAME:
      case TERMSANDCONDITIONS_NAME:
        dispatch('logMetrics', {
          screenShown: true,
          notificationsRegistered: false,
          didErrorAttemptingToUpdateStatus: true,
        });

        this.app.$router.push({ path: NOTIFICATIONS_GENERIC_FAILURE_PATH });
        break;
      case AUTH_RETURN_NAME:
        // if there is a fatal error when load is called during
        // the middleware we need to ensure the promise is
        // resolved and doesn't leave the user stuck
        resolveTask('');
        break;
      default:
        addApiError(this, 10002);
        break;
    }
  },
  addNotificationCookie() {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);

    const cookieExists = !!this.$cookies.get(`nhso.notifications-prompt-${decodedToken.sub}`);
    if (!cookieExists) {
      setCookie({
        cookies: this.$cookies,
        key: `nhso.notifications-prompt-${decodedToken.sub}`,
        value: decodedToken.sub,
        expires: '1y',
        secure: this.$env.SECURE_COOKIES,
      });
    }
  },
  checkNotificationCookie({ commit }) {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);

    const cookieExists = !!this.$cookies.get(`nhso.notifications-prompt-${decodedToken.sub}`);

    commit(SET_NOTIFICATION_COOKIE_EXISTS, cookieExists);
  },
};
