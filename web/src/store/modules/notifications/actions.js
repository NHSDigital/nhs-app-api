import NativeApp from '@/services/native-app';
import { SET_REGISTRATION, SET_SETTINGS_ENABLED, SET_WAITING } from './mutation-types';

const load = 'load';
const toggle = 'toggle';
let loadingComplete;

const completeLoading = () => {
  if (loadingComplete) {
    loadingComplete();
  }
};

export default {
  async authorised({ state, commit }, deviceResponse) {
    const { trigger, devicePns, deviceType } = JSON.parse(deviceResponse);
    let promise;

    if (trigger === toggle) {
      const registering = !state.registered;

      promise = registering
        ? this.app.$http.postV1ApiUsersDevices({ addDeviceRequest: { devicePns, deviceType } })
        : this.app.$http.deleteV1ApiUsersDevices({ devicePns });

      promise.then(() => commit(SET_REGISTRATION, registering))
        .finally(() => commit(SET_WAITING, false));
    } else {
      promise = this.app.$http.getV1ApiUsersDevices({ devicePns })
        .then(() => commit(SET_REGISTRATION, true))
        .catch(() => commit(SET_REGISTRATION, false))
        .finally(() => completeLoading());
    }
  },
  load() {
    const loading = new Promise((resolve) => {
      loadingComplete = resolve;
    });

    NativeApp.areNotificationsEnabled();
    return loading;
  },
  settingsEnabled({ commit }, isEnabled) {
    if (isEnabled) {
      NativeApp.requestPnsToken(load);
    } else {
      commit(SET_REGISTRATION, false);
      completeLoading();
    }
    commit(SET_SETTINGS_ENABLED, isEnabled);
  },
  toggle({ commit }) {
    commit(SET_WAITING, true);
    NativeApp.requestPnsToken(toggle);
  },
  unauthorised({ commit }) {
    commit(SET_WAITING, false);
    commit(SET_REGISTRATION, false);
  },
};
