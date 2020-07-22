import each from 'jest-each';
import canHandleBiometricsWeb from '@/lib/biometrics/canVersionHandleBiometricsWeb';
import getters from '@/store/modules/appVersion/getters';

describe('canHandleBiometricsWeb', () => {
  each([
    [false, '1.34.0', 'android'],
    [false, '1.35.0', 'android'],
    [true, '1.36.0', 'android'],
    [false, '1.34.0', 'ios'],
    [true, '1.35.0', 'ios'],
  ])
    .it('will return %s if the version is after %s and the device source is %s', (expectedResult, version, source) => {
      const page = {
        $store: {
          state: {
            device: {
              source,
              isNativeApp: true,
            },
            nativeVersion: version,
          },
        },
      };

      page.$store.getters = {
        'appVersion/isNativeVersionAfter': v => getters.isNativeVersionAfter(page.$store.state)(v),
      };

      expect(canHandleBiometricsWeb(page)).toBe(expectedResult);
    });
});
