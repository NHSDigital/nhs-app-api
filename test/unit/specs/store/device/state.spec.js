import { state } from '../../../../../src/store/device';

describe('state', () => {
  it('will set the default isNativeApp boolean to false', () => {
    expect(state().isNativeApp).toEqual(false);
  });
});
