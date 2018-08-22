import mutations from '../../../../../src/store/modules/pageTitle/mutations';
import { UPDATE_PAGE_TITLE } from '../../../../../src/store/modules/pageTitle/mutation-types';

describe('UPDATE_PAGE_TITLE', () => {
  it('will call the native app handle to update the pageTitle', () => {
    const mockUpdatePageTitleFunction = jest.fn();
    window.nativeApp = {
      updatePageTitle: mockUpdatePageTitleFunction,
    };
    process.client = true;
    const state = {
      pageTitle: '',
    };
    const newPageTitle = 'new page title';

    mutations[UPDATE_PAGE_TITLE](state, newPageTitle);

    expect(state.pageTitle).toEqual('');
    expect(mockUpdatePageTitleFunction).toHaveBeenCalledWith(newPageTitle);
  });

  it('will set the header text on the state to the sent value when no native app handles are present', () => {
    window.nativeApp = undefined;
    const state = {};
    const newPageTitle = 'new page title';

    mutations[UPDATE_PAGE_TITLE](state, newPageTitle);

    expect(state.pageTitle).toEqual(newPageTitle);
  });
});
