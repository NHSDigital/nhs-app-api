import ViewDecision from '@/pages/organ-donation/view-decision';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import YourDecision from '@/components/organ-donation/YourDecision';
import { initialState, DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

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

  const mountPage = () => mount(ViewDecision, { $store, $style, $t });

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(ViewDecision, { $store: store, $style });
  };

  beforeEach(() => {
    $store = createStore({ state: createState() });
    $style = {};
    wrapper = mountPage();
  });

  describe('YourDecision', () => {
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
  });
});
