import Confirmation from '@/components/organ-donation/Confirmation';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, shallowMount } from '../../helpers';

describe('confirmation', () => {
  let $store;
  let state;
  let wrapper;

  beforeEach(() => {
    state = {
      organDonation: initialState(),
    };
    $store = createStore({ state });
    wrapper = shallowMount(Confirmation, {
      $store,
      propsData: {
        submitAttempted: false,
      },
    });
  });

  describe('toggleAccuracy', () => {
    it('will dispatch `toggleAccuracyAcceptance`', () => {
      wrapper.vm.toggleAccuracy();
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/toggleAccuracyAcceptance');
    });
  });

  describe('togglePrivacy', () => {
    it('will dispatch `togglePrivacyAcceptance`', () => {
      wrapper.vm.togglePrivacy();
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/togglePrivacyAcceptance');
    });
  });
});
