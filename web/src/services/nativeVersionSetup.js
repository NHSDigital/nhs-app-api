/* eslint-disable no-underscore-dangle */
import NativeCallbacks from '@/services/native-app';

export default function (store) {
  if (process.client && store.state.device.isNativeApp && !store.state.appVersion.nativeVersion) {
    const appVersion = NativeCallbacks.fetchNativeAppVersion();
    /* This check is to support iOS as it will not respond with a value,
        instead it will send the value directly into the store.
        */
    if (appVersion) {
      store.dispatch('appVersion/updateNativeVersion', appVersion);
    }
  }
}
