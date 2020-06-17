import Symptoms from '@/pages/symptoms';
import { shallowMount } from '../../helpers';

describe('symptoms page', () => {
  it('will dispatch device/unlockNavBar when page mounted', () => {
    const $store = {
      dispatch: jest.fn(),
    };

    shallowMount(Symptoms, { $store });

    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });
});
