import { shallowMount } from '../../helpers';
import Symptoms from '@/pages/symptoms';

describe('symptoms page', () => {
  it('will dispatch device/unlockNavBar when page mounted', () => {
    const $store = {
      dispatch: jest.fn(),
    };

    shallowMount(Symptoms, { $store });

    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });
});
