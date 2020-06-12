export default function (page) {
  const isNativeVersionAfter = page.$store.getters['appVersion/isNativeVersionAfter'];

  return !page.$store.state.device.isNativeApp || isNativeVersionAfter('1.34.0');
}
