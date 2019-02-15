import MakeDecision from '@/components/organ-donation/MakeDecision';
import OrganDonationButton from '@/components/organ-donation/OrganDonationButton';
import { DECISION_OPT_IN, DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';
import { $t, mount } from '../../helpers';

describe('make decision', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mount(MakeDecision, { $t });
  });

  it('will translate the register subheader', () => {
    expect($t).toHaveBeenCalledWith('organDonation.register.subheader');
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

