import actions from '../../../../src/store/modules/pageTitle/actions';
import { UPDATE_PAGE_TITLE } from '../../../../src/store/modules/pageTitle/mutation-types';

const { updatePageTitle } = actions;

describe('updatePageTitle', () => {
  it('will call commit with the sent value', () => {
    const newPageTitle = 'new page title';

    const commit = jest.fn();

    updatePageTitle({ commit }, newPageTitle);

    expect(commit).toBeCalledWith(UPDATE_PAGE_TITLE, newPageTitle);
  });
});
