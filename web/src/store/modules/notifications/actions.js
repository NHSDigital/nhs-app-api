import get from 'lodash/fp/get';
import NativeApp from '@/services/native-app';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import jwt from 'jwt-decode';
import { NOTIFICATIONS_NAME, AUTH_RETURN_NAME } from '@/router/names';
import { NOTIFICATIONS_GENERIC_FAILURE_PATH } from '@/router/paths';
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

let resolveLoading;
let resolveChecking;

export default {
  authorised({ state, commit }, deviceResponse) {
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

      return promise.then(() => {
        commit(SET_REGISTRATION, registering);

        if (this.app.$router.currentRoute.name === NOTIFICATIONS_NAME) {
          this.dispatch('notifications/logMetrics',
            { screenShown: true,
              notificationsRegistered: registering,
            });
        }
      })
        // NHSO-7584
        .catch((error) => {
          if (this.app.$router.currentRoute.name === NOTIFICATIONS_NAME) {
            this.dispatch('notifications/logMetrics',
              { screenShown: true,
                notificationsRegistered: state.registered,
              });
            this.app.$router.push({ path: NOTIFICATIONS_GENERIC_FAILURE_PATH });
          } else {
            if (get('response.status')(error) === 404) {
              addApiError(this, 10003, error.message);
            }
            throw new Error(error.message);
          }
        })
        .finally(() => commit(SET_WAITING, false));
    }

    return this.app.$http.getV1ApiUsersMeDevices({ devicePns })
      .then(() => commit(SET_REGISTRATION, true))
      .catch(() => commit(SET_REGISTRATION, false))
      .finally(() => {
        resolveLoading(authorisationStatus.authorised);
      });
  },
  load() {
    const loading = new Promise((resolve) => {
      resolveLoading = resolve;
    });

    NativeApp.getNotificationsStatus();
    return loading;
  },
  logMetrics({ rootState }, params) {
    const { screenShown, notificationsRegistered } = params;
    const platform = rootState.device.source;

    this.app.$http.postV1ApiUsersMeDevicesPromptMetrics({
      notificationsPromptData:
        {
          screenShown,
          notificationsRegistered,
          platform,
        },
    }).catch(() => {
      // do nothing as this is just logging
    });
  },
  settingsStatus({ commit }, status) {
    switch (status) {
      case authorisationStatus.authorised:
        NativeApp.requestPnsToken(load);
        break;
      case authorisationStatus.denied:
        commit(SET_REGISTRATION, false);

        if (this.app.$router.history.pending !== null &&
          this.app.$router.history.pending.name === NOTIFICATIONS_NAME) {
          this.app.$router.push({ path: NOTIFICATIONS_GENERIC_FAILURE_PATH });
        } else {
          addApiError(this, 10001);
        }

        resolveLoading(status);
        break;
      default:
        commit(SET_REGISTRATION, false);
        resolveLoading(status);
    }
  },
  retryToggle({ dispatch }) {
    // NHSO-7584
    this.dispatch('errors/clearAllApiErrors');
    EventBus.$emit(UPDATE_HEADER, 'navigation.pages.headers.notifications');
    EventBus.$emit(UPDATE_TITLE, 'navigation.pages.headers.notifications');
    dispatch('toggle');
  },
  toggle({ commit }) {
    commit(SET_WAITING, true);
    commit(TOGGLE_UPDATED, true);

    NativeApp.requestPnsToken(toggle);
  },
  unauthorised({ commit }) {
    commit(SET_WAITING, false);
    if (this.app.$router.currentRoute.name === NOTIFICATIONS_NAME) {
      this.dispatch('notifications/logMetrics',
        { screenShown: true,
          notificationsRegistered: false,
        });
      this.app.$router.push({ path: NOTIFICATIONS_GENERIC_FAILURE_PATH });
    } else if (this.app.$router.currentRoute.name === AUTH_RETURN_NAME) {
      // if there is a fatal error when load is called during
      // the middleware we need to ensure the promise is
      // resolved and doesn't leave the user stuck
      resolveLoading('');
    } else {
      addApiError(this, 10002);
    }
  },
  deviceCookieExists({ commit }, exists) {
    commit(SET_NOTIFICATION_COOKIE_EXISTS, exists);
    resolveChecking(exists);
    resolveChecking = undefined;
  },
  addNotificationCookie() {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);
    NativeApp.addNotificationCookie(decodedToken.sub);
  },
  checkNotificationCookie() {
    const cookieValue = this.$cookies.get('nhso.session');
    const decodedToken = jwt(cookieValue.accessToken);

    const checking = new Promise((resolve) => {
      resolveChecking = resolve;
    });

    NativeApp.checkNotificationCookie(decodedToken.sub);

    return checking;
  },
};
