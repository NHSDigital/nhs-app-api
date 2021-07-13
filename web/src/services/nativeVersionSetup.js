/* eslint-disable no-underscore-dangle */
import NativeApp from '@/services/native-app';

export default function (store) {
  if (store.state.device.isNativeApp && !store.state.appVersion.nativeVersion) {
    const appVersion = NativeApp.fetchNativeAppVersion();
    /* This check is to support iOS as it will not respond with a value,
        instead it will send the value directly into the store.
        */
    if (appVersion) {
      store.dispatch('appVersion/updateNativeVersion', appVersion);
    }
  }
}
