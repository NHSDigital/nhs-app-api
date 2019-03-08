import MakeDecision from '@/components/organ-donation/MakeDecision';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import { DECISION_OPT_IN, DECISION_OPT_OUT, initialState } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

describe('make decision', () => {
  let wrapper;
  let $store;
  let state;

  const createState = () => ({
    organDonation: initialState(),
    device: {
      source: 'web',
    },
  });

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(MakeDecision, { $store: store, $t });
  };

  beforeEach(() => {
    state = createState();
    wrapper = mountWrapper(MakeDecision);
  });

  describe('registration journey subheader', () => {
    describe('not amending', () => {
      beforeEach(() => {
        state.organDonation.isAmending = false;
        wrapper = mountWrapper();
      });

      it('will translate the register subheader', () => {
        expect($t).toHaveBeenCalledWith('organDonation.register.subheaderRegister');
      });
    });

    describe('amending', () => {
      beforeEach(() => {
        state.organDonation.isAmending = true;
        wrapper = mountWrapper();
      });

      it('will translate the amend subheader', () => {
        expect($t).toHaveBeenCalledWith('organDonation.register.subheaderAmend');
      });
    });
  });

  describe('organ donation buttons', () => {
    let buttons;

    beforeEach(() => {
      buttons = wrapper.findAll(OrganDonationButton);
    });

    describe('no', () => {
      let noButton;

      beforeEach(() => {
        noButton = buttons.at(0);
      });

      it('will exist', () => {
        expect(noButton.exists()).toBe(true);
      });

      it('will have a value that sets the decision to "OptOut"', () => {
        const { decision } = noButton.props();
        expect(decision).toEqual(DECISION_OPT_OUT);
      });
    });

    describe('yes', () => {
      let yesButton;

      beforeEach(() => {
        yesButton = buttons.at(1);
      });

      it('will exist', () => {
        expect(yesButton.exists()).toBe(true);
      });

      it('will have a value that sets the decision to "OptIn"', () => {
        const { decision } = yesButton.props();
        expect(decision).toBe(DECISION_OPT_IN);
      });
    });
  });
});

