import actions from '@/store/modules/appVersion/actions';
import { UPDATE_WEB_VERSION } from '@/store/modules/appVersion/mutation-types';

const { updateWebVersion } = actions;

describe('updateWebVersion', () => {
  it('will remove a v from the beginning of the version, if followed immidiately by a number', () => {
    const originalVersion = 'v0.9.0';
    const expectedVersion = '0.9.0';

    const commit = jest.fn();

    updateWebVersion({ commit }, originalVersion);

    expect(commit).toBeCalledWith(UPDATE_WEB_VERSION, expectedVersion);
  });
});
