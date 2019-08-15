import Sources from '@/lib/sources';

export default function ({ store, route }) {
  const { source } = route.query;

  if (Sources.isNative(source)) {
    store.dispatch('device/updateIsNativeApp', true);
    store.dispatch('device/setSourceDevice', source);
  }
}
