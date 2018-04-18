import { updateHeaderText } from '@/store/modules/header/actions';
import { UPDATE_HEADER_TEXT } from '@/store/modules/header/mutation-types';

describe('updateHeaderText', () => {
  it('will call commit with the sent value', () => {
    const newHeaderValue = 'new page header';

    const commit = jest.fn();

    updateHeaderText({ commit }, newHeaderValue);

    expect(commit).toBeCalledWith(UPDATE_HEADER_TEXT, newHeaderValue);
  });
});
