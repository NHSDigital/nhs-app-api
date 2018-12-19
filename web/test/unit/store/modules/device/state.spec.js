import device from '@/store/modules/device/index';

const { state } = device;
describe('state', () => {
  it('will set the default isNativeApp boolean to false', () => {
    expect(state().isNativeApp).toEqual(false);
  });
});
