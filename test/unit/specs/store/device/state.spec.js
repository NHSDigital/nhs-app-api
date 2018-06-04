/* eslint-disable import/extensions */
import device from '../../../../../src/store/modules/device';

const { state } = device;
describe('state', () => {
  it('will set the default isNativeApp boolean to false', () => {
    expect(state().isNativeApp).toEqual(false);
  });
});
