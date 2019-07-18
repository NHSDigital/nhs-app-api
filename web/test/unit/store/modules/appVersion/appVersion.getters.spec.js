import getters from '@/store/modules/appVersion/getters';

describe('getters', () => {
  function setVersion(nativeVersion, webVersion) {
    return {
      appVersion: {
        nativeVersion,
        webVersion,
      },
    };
  }

  describe('isNativeVersionAfter', () => {
    const { isNativeVersionAfter } = getters;

    describe('production version 1.14.0', () => {
      let getter;

      beforeEach(() => {
        getter = isNativeVersionAfter(setVersion('1.14.0', '1.14.0'));
      });
      it('will return true for 1.13.0', () => {
        expect(getter('1.13.0')).toEqual(true);
      });

      it('will return true for 1.14.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.15.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.14.0-RC1', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for develop', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 2838 MR', () => {
        expect(getter('1.14.0')).toEqual(true);
      });
    });

    describe('RC version 1.14.0-RC4', () => {
      let getter;

      beforeEach(() => {
        getter = isNativeVersionAfter(setVersion('1.14.0-RC4', '1.14.0-RC4'));
      });

      it('will return true for 1.13.0', () => {
        expect(getter('1.13.0')).toEqual(true);
      });

      it('will return true for 1.14.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.15.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.14.0-RC5', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for develop', () => {
        expect(getter('develop')).toEqual(true);
      });

      it('will return false for 2838 MR', () => {
        expect(getter('2838')).toEqual(false);
      });
    });

    describe('develop number check', () => {
      let getter;

      beforeEach(() => {
        getter = isNativeVersionAfter(setVersion('develop', 'develop'));
      });

      it('will return true for 1.13.0', () => {
        expect(getter('1.13.0')).toEqual(true);
      });

      it('will return true for 1.14.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.15.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.14.0-RC5', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for develop', () => {
        expect(getter('develop')).toEqual(true);
      });

      it('will return false for 2838 MR', () => {
        expect(getter('2838')).toEqual(false);
      });
    });

    describe('2838 MR number check', () => {
      let getter;

      beforeEach(() => {
        getter = isNativeVersionAfter(setVersion('2838', '2838'));
      });

      it('will return true for 1.13.0', () => {
        expect(getter('1.13.0')).toEqual(true);
      });

      it('will return true for 1.14.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.15.0', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for 1.14.0-RC5', () => {
        expect(getter('1.14.0')).toEqual(true);
      });

      it('will return true for develop', () => {
        expect(getter('develop')).toEqual(true);
      });

      it('will return false for 2837', () => {
        expect(getter('2837')).toEqual(false);
      });

      it('will return false for 2838', () => {
        expect(getter('2838')).toEqual(false);
      });

      it('will return false for 2839', () => {
        expect(getter('2839')).toEqual(false);
      });
    });
  });
});
