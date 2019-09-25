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

    describe('production version 1.14.0', () => {
      let isVersionAfter;

      beforeEach(() => {
        isVersionAfter = isNativeVersionAfter(setVersion('1.14.0', '1.14.0'));
      });
      it('will return true for 1.13.0', () => {
        expect(isVersionAfter('1.13.0')).toEqual(true);
      });

      it('will return false for 1.14.0', () => {
        expect(isVersionAfter('1.14.0')).toEqual(false);
      });

      it('will return false for 1.15.0', () => {
        expect(isVersionAfter('1.15.0')).toEqual(false);
      });

      it('will return false for 1.14.0-RC1', () => {
        expect(isVersionAfter('1.14.0-RC1')).toEqual(false);
      });

      it('will return false for develop', () => {
        expect(isVersionAfter('develop')).toEqual(false);
      });

      it('will return false for 2838 MR', () => {
        expect(isVersionAfter('2838')).toEqual(false);
      });
    });

    describe('RC version 1.14.0-RC4', () => {
      let isVersionAfter;

      beforeEach(() => {
        isVersionAfter = isNativeVersionAfter(setVersion('1.14.0-RC4', '1.14.0-RC4'));
      });

      it('will return true for 1.13.0', () => {
        expect(isVersionAfter('1.13.0')).toEqual(true);
      });

      it('will return false for 1.14.0', () => {
        expect(isVersionAfter('1.14.0')).toEqual(false);
      });

      it('will return false for 1.15.0', () => {
        expect(isVersionAfter('1.15.0')).toEqual(false);
      });

      it('will return false for 1.14.0-RC5', () => {
        expect(isVersionAfter('1.14.0-RC5')).toEqual(false);
      });

      it('will return false for develop', () => {
        expect(isVersionAfter('develop')).toEqual(false);
      });

      it('will return false for 2838 MR', () => {
        expect(isVersionAfter('2838')).toEqual(false);
      });
    });

    describe('develop number check', () => {
      let isVersionAfter;

      beforeEach(() => {
        isVersionAfter = isNativeVersionAfter(setVersion('develop', 'develop'));
      });

      it('will return true for 1.13.0', () => {
        expect(isVersionAfter('1.13.0')).toEqual(true);
      });

      it('will return true for 1.14.0', () => {
        expect(isVersionAfter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.15.0', () => {
        expect(isVersionAfter('1.15.0')).toEqual(true);
      });

      it('will return true for 1.14.0-RC5', () => {
        expect(isVersionAfter('1.14.0-RC5')).toEqual(true);
      });

      it('will return false for develop', () => {
        expect(isVersionAfter('develop')).toEqual(false);
      });

      it('will return false for 2838 MR', () => {
        expect(isVersionAfter('2838')).toEqual(false);
      });
    });

    describe('2838 MR number check', () => {
      let isVersionAfter;

      beforeEach(() => {
        isVersionAfter = isNativeVersionAfter(setVersion('2838', '2838'));
      });

      it('will return true for 1.13.0', () => {
        expect(isVersionAfter('1.13.0')).toEqual(true);
      });

      it('will return true for 1.14.0', () => {
        expect(isVersionAfter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.15.0', () => {
        expect(isVersionAfter('1.15.0')).toEqual(true);
      });

      it('will return true for 1.14.0-RC5', () => {
        expect(isVersionAfter('1.14.0-RC5')).toEqual(true);
      });

      it('will return true for develop', () => {
        expect(isVersionAfter('develop')).toEqual(true);
      });

      it('will return true for 2837', () => {
        expect(isVersionAfter('2837')).toEqual(true);
      });

      it('will return false for 2838', () => {
        expect(isVersionAfter('2838')).toEqual(false);
      });

      it('will return false for 2839', () => {
        expect(isVersionAfter('2839')).toEqual(false);
      });
    });
  });
});
