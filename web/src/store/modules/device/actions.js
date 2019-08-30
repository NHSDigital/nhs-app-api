import Sources from '@/lib/sources';
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE, GO_TO_CHECK_SYMPTOMS, GO_TO_GP_FINDER } from './mutation-types';
import NativeCallbacks from '@/services/native-app';

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
  goToGPFinder({ commit }) {
    commit(GO_TO_GP_FINDER);
  },
  setSourceDevice({ commit }, source = Sources.Web) {
    commit(SET_SOURCE_DEVICE, source);
  },
  unlockNavBar() {
    if (process.client) {
      NativeCallbacks.pageLoadComplete();
    }
  },
};
