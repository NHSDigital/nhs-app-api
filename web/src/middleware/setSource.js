import Sources from '@/lib/sources';

export default function (context) {
  const { to, store, next } = context;

  const { userAgent } = navigator;
  let { source } = to.query;

  if (/nhsapp-android/i.test(userAgent)) {
    source = Sources.Android;
  } else if (/nhsapp-ios/i.test(userAgent)) {
    source = Sources.iOS;
  }

  if (Sources.isNative(source)) {
    store.dispatch('device/updateIsNativeApp', true);
    store.dispatch('device/setSourceDevice', source);
  }

  return next();
}
