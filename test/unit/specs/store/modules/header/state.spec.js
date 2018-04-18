import state from '@/store/modules/header/state';

describe('state', () => {
  it('will set the header text to an empty string', () => {
    expect(state.headerText).toEqual('');
  });
});
