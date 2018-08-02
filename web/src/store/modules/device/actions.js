import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, GO_TO_CHECK_SYMPTOMS } from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT_DEVICE);
  },
  updateIsNativeApp({ commit }, isNativeApp) {
    commit(UPDATE_IS_NATIVE_APP, isNativeApp);
  },
  goToCheckSymptoms({ commit }) {
    commit(GO_TO_CHECK_SYMPTOMS);
  },
  setSourceDevice({ commit }, source = 'web') {
    commit(SET_SOURCE_DEVICE, source);
  },

};
