import Confirmation from '@/components/organ-donation/Confirmation';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, mount, shallowMount } from '../../helpers';

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

  describe('Confirmation checkbox rendering', () => {
    beforeEach(() => {
      const mountConfirmation = () =>
        mount(Confirmation, {
          state: {
            organDonation: {
              isAccuracyAccepted: false,
              isPrivacyAccepted: false,
            },
          },
          propsData: {
            submitAttempted: false,
          },
        });

      wrapper = mountConfirmation();
    });

    it('will verify an associated label for the confirmation on your decision checkbox', () => {
      expect(wrapper.find("input[type='checkbox'][id='accuracy-accuracy-checkbox']")
        .exists()).toEqual(true);

      expect(wrapper.find("label[for='accuracy-accuracy-checkbox']")
        .exists()).toEqual(true);
    });

    it('will verify an associated label for the privacy checkbox', () => {
      expect(wrapper.find("input[type='checkbox'][id='privacy-privacy-checkbox']")
        .exists()).toEqual(true);

      expect(wrapper.find("label[for='privacy-privacy-checkbox']")
        .exists()).toEqual(true);
    });
  });
});
