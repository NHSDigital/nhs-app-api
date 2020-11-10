import NativeReferrerSetup from '@/services/nativeReferrerSetup';

describe('Native Referrer Setup', () => {
  describe('native android', () => {
    let $store;
    let dispatch;
    beforeEach(() => {
      dispatch = jest.fn();
      global.nativeApp = {
        fetchNativeAppReferrer: jest.fn(),
      };

      $store = {
        state: {
          device: {
            source: 'android',
            referrer: undefined,
            isNativeApp: true,
          },
        },
        dispatch,
      };
      global.nativeApp.fetchNativeAppReferrer = () => 'test';
      NativeReferrerSetup($store);
    });

    it('will update referrer', () => {
      expect(dispatch).toHaveBeenCalledWith('device/updateReferrer', 'test');
    });
  });

  describe('native ios', () => {
    let $store;
    let dispatch;
    beforeEach(() => {
      dispatch = jest.fn();

      $store = {
        state: {
          device: {
            source: 'ios',
            referrer: undefined,
            isNativeApp: true,
          },
        },
        dispatch,
      };
      NativeReferrerSetup($store);
    });

    it('will not call to update referrer', () => {
      expect(dispatch).not.toHaveBeenCalled();
    });
  });
});
