import Sources from '@/lib/sources';
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE } from './mutation-types';
import NativeCallbacks from '@/services/native-app';

export default {
  init({ commit }) {
    commit(INIT_DEVICE);
  },
  updateIsNativeApp({ commit }, isNativeApp) {
    commit(UPDATE_IS_NATIVE_APP, isNativeApp);
  },
  setSourceDevice({ commit }, source = Sources.Web) {
    commit(SET_SOURCE_DEVICE, source);
  },
  unlockNavBar() {
    if (process.client) {
      NativeCallbacks.pageLoadComplete();
      window.nhsAppPageLoadComplete = true;
    }
  },
  pageLoadComplete() {
    if (process.client) {
      NativeCallbacks.pageLoadComplete();
      window.nhsAppPageLoadComplete = true;
    }
  },
};
