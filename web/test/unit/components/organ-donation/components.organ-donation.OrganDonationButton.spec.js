import i18n from '@/plugins/i18n';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import {
  ORGAN_DONATION_ADDITIONAL_DETAILS_PATH,
  ORGAN_DONATION_YOUR_CHOICE_PATH,
} from '@/router/paths';
import {
  DECISION_OPT_OUT,
  DECISION_OPT_IN,
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import * as dependency from '@/lib/utils';
import { createStore, mount } from '../../helpers';

describe('organ donation button', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    dependency.redirectTo = jest.fn();
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
        mountOpts: { i18n },
      });
    });

    it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
      wrapper.find('button').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', DECISION_OPT_OUT);
    });

    describe('text translations', () => {
      it('will display the no button', () => {
        expect(wrapper.text()).toBe('NO I do not want to donate my organs');
      });
    });

    describe('computed properties', () => {
      describe('nextRoute', () => {
        it('will be the ORGAN_DONATION path', () => {
          expect(wrapper.vm.nextRoute).toEqual(ORGAN_DONATION_ADDITIONAL_DETAILS_PATH);
        });
      });
    });
  });

  describe('yes state', () => {
    beforeEach(() => {
      wrapper = mount(OrganDonationButton, {
        $store,
        propsData: { decision: DECISION_OPT_IN },
        mountOpts: { i18n },
      });
    });

    it('will dispatch `makeDecision` with the supplied decision when clicked', () => {
      wrapper.find('button').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/makeDecision', DECISION_OPT_IN);
    });

    describe('text translations', () => {
      it('will display the yes button', () => {
        expect(wrapper.text()).toBe('YES I want to donate all or some of my organs');
      });
    });

    describe('computed properties', () => {
      describe('nextRoute', () => {
        it('will be the ORGAN_DONATION_YOUR_CHOICE path', () => {
          expect(wrapper.vm.nextRoute).toEqual(ORGAN_DONATION_YOUR_CHOICE_PATH);
        });
      });
    });

    describe('data', () => {
      it('will set the data based on the decision', () => {
        const stateData = wrapper.vm;
        expect(stateData.nextRoute).toEqual(ORGAN_DONATION_YOUR_CHOICE_PATH);
        expect(stateData.style).toEqual(wrapper.vm.$style['yes-button']);
        expect(stateData.headerKey).toEqual('organDonation.button.iDoWantToDonate.header');
        expect(stateData.subHeaderKey).toEqual('organDonation.button.iDoWantToDonate.subheader');
        expect(stateData.icon).toEqual(YesIcon);
      });
    });
  });
});
