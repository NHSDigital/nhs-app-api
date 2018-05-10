import state from '@/store/modules/device/state';

describe('state', () => {
  it('will set the default isNativeApp boolean to false', () => {
    expect(state.isNativeApp).toEqual(false);
  });
});
