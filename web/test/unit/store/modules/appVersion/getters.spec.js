import getters from '@/store/modules/appVersion/getters';

describe('getters', () => {
  describe('isPreForceUpdate', () => {
    const { isPreForceUpdate } = getters;

    it('will be false if hideMenuBar is a function', () => {
      window.nativeApp = {
        hideMenuBar: jest.fn(),
      };
      expect(isPreForceUpdate()).toEqual(false);
    });

    it('will be true if hideMenuBar is not a function', () => {
      window.nativeApp = {};
      expect(isPreForceUpdate()).toEqual(true);
    });
  });
});
