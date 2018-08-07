import header from '@/store/modules/header/index';

const { state } = header;

describe('state', () => {
  it('will set the header text to an empty string', () => {
    expect(state().headerText).toEqual('');
  });
});
