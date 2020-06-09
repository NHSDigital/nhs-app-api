import get from 'lodash/fp/get';
import NativeApp from '@/services/native-app';
import { SET_REGISTRATION, SET_WAITING } from './mutation-types';

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
        ? this.app.$http.postV1ApiUsersDevices({ addDeviceRequest: { devicePns, deviceType } })
        : this.app.$http.deleteV1ApiUsersDevices({ devicePns });

      return promise.then(() => commit(SET_REGISTRATION, registering))
        // NHSO-7584
        .catch((error) => {
          if (get('response.status')(error) === 404) {
            addApiError(this, 10003, error.message);
          }
          throw new Error(error.message);
        })
        .finally(() => commit(SET_WAITING, false));
    }

    return this.app.$http.getV1ApiUsersDevices({ devicePns })
      .then(() => commit(SET_REGISTRATION, true))
      .catch(() => commit(SET_REGISTRATION, false))
      .finally(() => resolveLoading(authorisationStatus.authorised));
  },
  load() {
    const loading = new Promise((resolve) => {
      resolveLoading = resolve;
    });

    NativeApp.getNotificationsStatus();
    return loading;
  },
  settingsStatus({ commit }, status) {
    switch (status) {
      case authorisationStatus.authorised:
        NativeApp.requestPnsToken(load);
        break;
      case authorisationStatus.denied:
        commit(SET_REGISTRATION, false);
        addApiError(this, 10001);
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
    this.dispatch('header/updateHeaderText', this.app.i18n.tc('pageHeaders.notifications'));
    this.dispatch('pageTitle/updatePageTitle', this.app.i18n.tc('pageHeaders.notifications'));
    dispatch('toggle');
  },
  toggle({ commit }) {
    commit(SET_WAITING, true);
    NativeApp.requestPnsToken(toggle);
  },
  unauthorised({ commit }) {
    commit(SET_WAITING, false);
    addApiError(this, 10002);
  },
};
