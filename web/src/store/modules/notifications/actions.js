import NativeApp from '@/services/native-app';
import { SET_REGISTRATION, SET_WAITING } from './mutation-types';

export default {
  async authorised({ state, commit }, deviceResponse) {
    const param = {
      addDeviceRequest: JSON.parse(deviceResponse),
    };
    await this.app.$http.postV1ApiUsersDevices(param).then(() => {
      commit(SET_REGISTRATION, !state.registered);
    }).finally(() => {
      commit(SET_WAITING, false);
    });
  },
  toggle({ commit }) {
    commit(SET_WAITING, true);
    NativeApp.requestPnsToken();
  },
  unAuthorised({ commit }) {
    commit(SET_WAITING, false);
  },
};
