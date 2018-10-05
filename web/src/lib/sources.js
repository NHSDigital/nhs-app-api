const Web = 'web';
const iOS = 'ios';
const Android = 'android';
const isNative = source => source === iOS || source === Android;

export default {
  Web,
  iOS,
  Android,
  isNative,
};
