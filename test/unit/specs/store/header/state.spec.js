/* eslint-disable import/extensions */
import header from '../../../../../src/store/modules/header';

const { state } = header;

describe('state', () => {
  it('will set the header text to an empty string', () => {
    expect(state().headerText).toEqual('');
  });
});
