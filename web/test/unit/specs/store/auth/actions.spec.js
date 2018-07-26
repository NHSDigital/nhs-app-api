import actions from '../../../../../src/store/modules/auth/actions';
import { UPDATE_CONFIG } from '../../../../../src/store/modules/auth/mutation-types';

const { updateConfig } = actions;

describe('updateConfig', () => {
  it('will call commit with the sent value', () => {
    const newConfigValue = { test: 'value' };

    const commit = jest.fn();

    updateConfig({ commit }, newConfigValue);

    expect(commit).toBeCalledWith(UPDATE_CONFIG, newConfigValue);
  });
});
