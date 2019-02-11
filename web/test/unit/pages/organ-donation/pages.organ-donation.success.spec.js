import ViewDecision from '@/pages/organ-donation/view-decision';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  DECISION_OPT_IN,
  initialState,
  STATE_OK,
  STATE_CONFLICTED,
} from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('view decision', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (state = {
    organDonation: initialState(),
    device: {
      source: 'web',
    },
  }) => state;

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(ViewDecision, { $store: store, $style });
  };

  describe('YourDecision', () => {
    beforeEach(() => {
      const state = createState();
      state.organDonation.registration.decision = DECISION_OPT_IN;
      state.organDonation.registration.decisionDetails.all = true;
      state.organDonation.registration.state = STATE_OK;
      $store = createStore({ state });
      wrapper = mountWrapper();
    });
    it('will exist', () => {
      expect(wrapper.find(YourDecision).exists()).toBe(true);
    });
  });

  describe('decision details', () => {
    describe('selected all organs', () => {
      beforeEach(() => {
        const state = createState();
        state.organDonation.registration.decision = DECISION_OPT_IN;
        state.organDonation.registration.decisionDetails.all = true;
        state.organDonation.registration.state = STATE_OK;
        $store = createStore({ state });
        wrapper = mountWrapper();
      });

      it('will show the opt-in decision text', () => {
        const yourDecision = wrapper.find(YourDecision);
        expect(yourDecision.text())
          .toContain('translate_organDonation.reviewYourDecision.yourDecision.optinDecisionText');
      });

      it('will not show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
      });
    });

    describe('selected some organs', () => {
      beforeEach(() => {
        const state = createState();
        state.organDonation.registration.decision = DECISION_OPT_IN;
        state.organDonation.registration.decisionDetails.all = false;
        state.organDonation.registration.state = STATE_OK;
        $store = createStore({ state });
        wrapper = mountWrapper();
      });

      it('will show the opt-in some decision text', () => {
        const yourDecision = wrapper.find(YourDecision);
        expect(yourDecision.text())
          .toContain('translate_organDonation.reviewYourDecision.yourDecision.optinSomeDecisionText');
      });

      it('will show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(true);
      });
    });
    describe('Decision submitted conflicted state', () => {
      beforeEach(() => {
        const state = createState();
        state.organDonation.registration.decision = DECISION_OPT_IN;
        state.organDonation.registration.decisionDetails.all = false;
        state.organDonation.registration.state = STATE_CONFLICTED;
        state.organDonation.registration.identifier = '12345';
        $store = createStore({ state });
        wrapper = mountWrapper();
      });

      it('will show the Decision submitted dialog text', () => {
        expect(wrapper.text())
          .toContain('translate_organDonation.viewDecision.decisionSubmitted.dialogText');
      });

      it('will show the Decision submitted message text', () => {
        expect(wrapper.text())
          .toContain('translate_organDonation.viewDecision.decisionSubmitted.messageText');
      });
    });
  });
});
