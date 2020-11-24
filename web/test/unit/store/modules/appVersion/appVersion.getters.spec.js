import each from 'jest-each';
import getters from '@/store/modules/appVersion/getters';

describe('getters', () => {
  function setVersion(nativeVersion, webVersion) {
    return {
      nativeVersion,
      webVersion,
    };
  }

  describe('isNativeVersionAfter', () => {
    const { isNativeVersionAfter } = getters;

    each([
      [false, undefined, '1.37.0'],
      [false, null, '1.37.0'],
      [false, '', '1.37.0'],
      [false, '1.40.5', '1.40.5'],
      [false, '1.41.0', '1.40.0'],
      [false, '1.41.3', '1.40.3'],
      [false, '1.40.x', '1.40.3'],
      [false, '1.40.x', '1.40.9999'],
      [false, '1.44.x', '1.44.99'],
      [true, '1.37.0', ''],
      [true, '1.37.0', undefined],
      [true, '1.37.0', null],
      [true, '1.40.x', '1.41.0'],
      [true, '1.40.x', '1.41.6'],
      [true, '1.44.0', 'develop'],
      [true, '1.21.0', 'pr2883'],
      [true, '1.40.0', '1.40.3'],
      [true, '1.42.0', '1.43.0'],
      [true, '1.42.6', '1.43.2'],
      [true, '2.41.0', '2.50.0'],
      [true, '2.41.8', '3.10.5'],
    ]).it('will return %s for \'%s\' when production version is \'%s\'', (expected, versionToCheck, productionVersion) => {
      const isVersionAfter = isNativeVersionAfter(
        setVersion(productionVersion, productionVersion),
      );

      expect(isVersionAfter(versionToCheck)).toEqual(expected);
    });
  });
});
