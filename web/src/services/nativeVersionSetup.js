/* eslint-disable no-underscore-dangle */
import NativeCallbacks from '@/services/native-app';
import Sources from '@/lib/sources';

export default function (store, route) {
  if (process.client) {
    const { source } = route.query;
    if (Sources.isNative(source)) {
      if (!store.state.appVersion.nativeVersion) {
        const appVersion = NativeCallbacks.fetchNativeAppVersion();
        /* This check is to support iOS as it will not respond with a value,
        instead it will send the value directly into the store.
        */
        if (appVersion) {
          store.dispatch('appVersion/updateNativeVersion', appVersion);
        }
      }
    }
  }
}
