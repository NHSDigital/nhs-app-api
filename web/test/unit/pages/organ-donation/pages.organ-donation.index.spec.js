import OrganDonation from '@/pages/organ-donation';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import {
  DECISION_NOT_FOUND,
  DECISION_OPT_OUT,
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

const createState = (decision = DECISION_NOT_FOUND) => {
  const state = {
    organDonation: initialState(),
  };

  state.organDonation.registration.decision = decision;

  return state;
};

describe('organ donation index page', () => {
  let $store;
  let $style;
  let wrapper;

  const mountOrganDonation = () => mount(OrganDonation, { $store, $style, $t });

  beforeEach(() => {
    $store = createStore({ state: createState() });
    $style = {
      button: 'button',
      grey: 'grey',
    };
    wrapper = mountOrganDonation();
  });

  it('will translate the register subheader', () => {
    expect($t).toHaveBeenCalledWith('organDonation.register.subheader');
  });

  describe('async data', () => {
    it('will dispatch the "organDonation/getRegistration" action', () => {
      wrapper.vm.$options.asyncData({ store: $store });
      expect($store.dispatch).toHaveBeenCalledWith('organDonation/getRegistration');
    });
  });

  describe('computed', () => {
    describe('noDecision', () => {
      it('will equal DECISION_OPT_OUT', () => {
        expect(wrapper.vm.noDecision).toEqual(DECISION_OPT_OUT);
      });
    });
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
});
