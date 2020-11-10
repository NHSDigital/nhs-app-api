/* eslint-disable no-underscore-dangle */
import NativeCallbacks from '@/services/native-app';

export default function (store) {
  if (store.state.device.source === 'android' && store.state.device.referrer === undefined) {
    if (!store.state.device.isNativeApp) {
      return;
    }
    const referrer = NativeCallbacks.fetchNativeAppReferrer();
    if (referrer) {
      store.dispatch('device/updateReferrer', referrer);
    }
  }
}
