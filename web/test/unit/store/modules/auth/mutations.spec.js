import mutations from '@/store/modules/auth/mutations';
import { UPDATE_CONFIG } from '@/store/modules/auth/mutation-types';

describe('UPDATE_CONFIG', () => {
  it('will set the config value on the state to the sent value', () => {
    const state = {};
    const newConfigValue = { test: 'value' };

    mutations[UPDATE_CONFIG](state, newConfigValue);

    expect(state.config).toEqual(newConfigValue);
  });

  it('repeated calls to set the config value on the state will keep updating its value', () => {
    const state = {};
    const newConfigValue = { test: 'value' };
    const secondNewConfigValue = { test: 'value' };

    mutations[UPDATE_CONFIG](state, newConfigValue);
    expect(state.config).toEqual(newConfigValue);

    mutations[UPDATE_CONFIG](state, secondNewConfigValue);
    expect(state.config).toEqual(secondNewConfigValue);
  });
});
