import header from '@/store/modules/header/index';

const { state } = header;

describe('state', () => {
  it('will set the miniMenuExpanded to be false', () => {
    expect(state().miniMenuExpanded).toEqual(false);
  });
});
