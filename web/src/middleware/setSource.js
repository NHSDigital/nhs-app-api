import Sources from '@/lib/sources';

export default function (context) {
  const userAgent = process.server ? context.req.headers['user-agent'] : navigator.userAgent;
  let { source } = context.route.query;

  if (/nhsapp-android/i.test(userAgent)) {
    source = Sources.Android;
  } else if (/nhsapp-ios/i.test(userAgent)) {
    source = Sources.iOS;
  }

  if (Sources.isNative(source)) {
    context.store.dispatch('device/updateIsNativeApp', true);
    context.store.dispatch('device/setSourceDevice', source);
  }
}
