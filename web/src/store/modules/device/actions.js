import Sources from '@/lib/sources';
import NativeCallbacks from '@/services/native-app';
import { INIT_DEVICE, UPDATE_IS_NATIVE_APP, SET_SOURCE_DEVICE } from './mutation-types';

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
    NativeCallbacks.pageLoadComplete();
    window.nhsAppPageLoadComplete = true;
  },
  pageLoadComplete() {
    NativeCallbacks.pageLoadComplete();
    window.nhsAppPageLoadComplete = true;
  },
};
