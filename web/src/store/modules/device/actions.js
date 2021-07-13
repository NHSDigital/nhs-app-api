import Sources from '@/lib/sources';
import NativeApp from '@/services/native-app';
import { INIT_DEVICE,
  UPDATE_IS_NATIVE_APP,
  SET_SOURCE_DEVICE,
  SET_APP_REFERRER } from './mutation-types';

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
    NativeApp.pageLoadComplete();
    window.nhsAppPageLoadComplete = true;
  },
  pageLoadComplete() {
    NativeApp.pageLoadComplete();
    window.nhsAppPageLoadComplete = true;
  },
  updateReferrer({ commit, rootState }, referrer) {
    const platform = rootState.device.source;

    if (platform === 'android') {
      commit(SET_APP_REFERRER, referrer);
    }
  },
};
