/* eslint-disable no-underscore-dangle */
import NativeApp from '@/services/native-app';

export default function (store) {
  if (store.state.device.source === 'android' && store.state.device.referrer === undefined) {
    if (!store.state.device.isNativeApp) {
      return;
    }
    const referrer = NativeApp.fetchNativeAppReferrer();
    if (referrer) {
      store.dispatch('device/updateReferrer', referrer);
    }
  }
}
