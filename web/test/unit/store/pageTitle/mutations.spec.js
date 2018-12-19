import mutations from '../../../../src/store/modules/pageTitle/mutations';
import { UPDATE_PAGE_TITLE } from '../../../../src/store/modules/pageTitle/mutation-types';

describe('UPDATE_PAGE_TITLE', () => {
  it('will set the header text on the state to the sent value when no native app handles are present', () => {
    window.nativeApp = undefined;
    const state = {};
    const newPageTitle = 'new page title';

    mutations[UPDATE_PAGE_TITLE](state, newPageTitle);

    expect(state.pageTitle).toEqual(newPageTitle);
  });
});
