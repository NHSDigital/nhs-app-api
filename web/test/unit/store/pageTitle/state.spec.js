/* eslint-disable import/extensions */
import pageTitle from '../../../../src/store/modules/pageTitle';

const { state } = pageTitle;

describe('state', () => {
  it('will set the page title to an empty string', () => {
    expect(state().pageTitle).toEqual('');
  });
});
