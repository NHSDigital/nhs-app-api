export default function (page) {
  const isNativeVersionAfter = page.$store.getters['appVersion/isNativeVersionAfter'];
  const { device } = page.$store.state;

  if (device.source === 'android') {
    return isNativeVersionAfter('1.35.0');
  }

  return !device.isNativeApp || isNativeVersionAfter('1.34.0');
}
