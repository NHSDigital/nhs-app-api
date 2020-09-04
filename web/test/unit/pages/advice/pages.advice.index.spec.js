import Advice from '@/pages/advice';
import { shallowMount } from '../../helpers';

describe('advice page', () => {
  it('will dispatch device/unlockNavBar when page mounted', () => {
    const $store = {
      dispatch: jest.fn(),
    };

    shallowMount(Advice, { $store });

    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });
});
