import OrganDonation from '@/pages/organ-donation';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import {
  DECISION_NOT_FOUND,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

const createState = (decision = DECISION_NOT_FOUND) => {
  const state = {
    organDonation: initialState(),
  };

  state.organDonation.registration.decision = decision;

  return state;
};

describe('organ donation index page', () => {
  let $store;
  let $t;
  let wrapper;

  beforeEach(() => {
    $t = jest.fn();
    $store = createStore({ state: createState() });
    wrapper = mount(OrganDonation, { $store, $t });
  });

  describe('async data', () => {
    it('will dispatch the "organDonation/getRegistration" action', () => {
      wrapper.vm.$options.asyncData({ store: $store });
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/getRegistration');
    });
  });

  describe('computed', () => {
    beforeEach(() => {
    });

    describe('noDecision', () => {
      it('will equal DECISION_OPT_OUT', () => {
        expect(wrapper.vm.noDecision).toEqual(DECISION_OPT_OUT);
      });
    });
  });

  describe('decision is irrelevant or not set', () => {
    beforeEach(() => {
      $store = createStore({ state: createState(DECISION_NOT_FOUND) });
      wrapper = mount(OrganDonation, { $store, $t });
    });

    it('will translate the register subheader', () => {
      expect($t).toHaveBeenCalledWith('organDonation.register.subheader');
    });

    describe('organ donation "no" button', () => {
      let organDonationButton;

      beforeEach(() => {
        organDonationButton = wrapper.find(OrganDonationButton);
      });

      it('will exist', () => {
        expect(organDonationButton.exists()).toBe(true);
      });

      it('will have a value that sets the decision to "OptOut"', () => {
        const { decision } = organDonationButton.props();
        expect(decision).toEqual(DECISION_OPT_OUT);
      });
    });

    describe('hasMadeDecision', () => {
      it('will be false', () => {
        expect(wrapper.vm.hasMadeDecision).toBe(false);
      });
    });
  });

  describe('decision is "OptOut"', () => {
    beforeEach(() => {
      $store = createStore({ state: createState(DECISION_OPT_OUT) });
      wrapper = mount(OrganDonation, { $store, $t });
    });

    it('will translate the additional details subheader', () => {
      expect($t).toHaveBeenCalledWith('organDonation.additionalDetails.subheader');
    });

    describe('hasMadeDecision', () => {
      it('will be true', () => {
        expect(wrapper.vm.hasMadeDecision).toBe(true);
      });
    });
  });

  describe('decision is "OptIn"', () => {
    beforeEach(() => {
      $store = createStore({ state: createState(DECISION_OPT_IN) });
      wrapper = mount(OrganDonation, { $store });
    });

    describe('hasMadeDecision', () => {
      it('will be true', () => {
        expect(wrapper.vm.hasMadeDecision).toBe(true);
      });
    });
  });
});
