import device from '@/store/modules/modal/index';

const { state } = device;
describe('state', () => {
  it('will set the default visibility to boolean to false', () => {
    expect(state().config.visible).toEqual(false);
  });
});
