import { state } from '../../../../../src/store/header';

describe('state', () => {
  it('will set the header text to an empty string', () => {
    expect(state().headerText).toEqual('');
  });
});
