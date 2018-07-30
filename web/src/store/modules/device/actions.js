import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT_DEVICE);
  },
  updateIsNativeApp({ commit }, isNativeApp) {
    commit(UPDATE_IS_NATIVE_APP, isNativeApp);
  },
  setSourceDevice({ commit }, source = 'web') {
    commit(SET_SOURCE_DEVICE, source);
  },
};
