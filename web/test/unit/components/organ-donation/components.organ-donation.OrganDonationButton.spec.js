/* eslint-disable object-curly-newline */
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import { ORGAN_DONATION_ADDITIONAL_DETAILS, ORGAN_DONATION_YOUR_CHOICE } from '@/lib/routes';
import {
  DECISION_OPT_OUT,
  DECISION_OPT_IN,
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('organ donation button', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });
  });

  describe('no state', () => {
    beforeEach(() => {
      wrapper = mount(OrganDonationButton, {
        $store,
        propsData: { decision: DECISION_OPT_OUT },
      });
    });

    it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
      wrapper.find('button').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', DECISION_OPT_OUT);
    });

    describe('text translations', () => {
      it('will display the no button header', () => {
        expect(wrapper.text()).toContain('translate_organDonation.register.noButton.header');
      });

      it('will display the no button subheader', () => {
        expect(wrapper.text()).toContain('translate_organDonation.register.noButton.subheader');
      });
    });

    describe('computed properties', () => {
      describe('nextRoute', () => {
        it('will be the ORGAN_DONATION path', () => {
          expect(wrapper.vm.nextRoute).toEqual(ORGAN_DONATION_ADDITIONAL_DETAILS.path);
        });
      });
    });
  });

  describe('yes state', () => {
    beforeEach(() => {
      wrapper = mount(OrganDonationButton, {
        $store,
        propsData: { decision: DECISION_OPT_IN },
      });
    });

    it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
      wrapper.find('button').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', DECISION_OPT_IN);
    });

    describe('text translations', () => {
      it('will display the yes button header', () => {
        expect(wrapper.text()).toContain('translate_organDonation.register.yesButton.header');
      });

      it('will display the yes button subheader', () => {
        expect(wrapper.text()).toContain('translate_organDonation.register.yesButton.subheader');
      });
    });

    describe('computed properties', () => {
      describe('nextRoute', () => {
        it('will be the ORGAN_DONATION_YOUR_CHOICE path', () => {
          expect(wrapper.vm.nextRoute).toEqual(ORGAN_DONATION_YOUR_CHOICE.path);
        });
      });
    });

    describe('data', () => {
      it('will set the data based on the decision', () => {
        const stateData = wrapper.vm;
        expect(stateData.nextRoute).toEqual(ORGAN_DONATION_YOUR_CHOICE.path);
        expect(stateData.style).toEqual(wrapper.vm.$style['yes-button']);
        expect(stateData.headerKey).toEqual('organDonation.register.yesButton.header');
        expect(stateData.subHeaderKey).toEqual('organDonation.register.yesButton.subheader');
        expect(stateData.icon).toEqual(YesIcon);
      });
    });
  });
});
