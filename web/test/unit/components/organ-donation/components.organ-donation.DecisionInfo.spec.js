import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import DecisionInfo from '@/components/organ-donation/DecisionInfo';
import Withdrawing from '@/components/organ-donation/Withdrawing';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN, DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';
import { mount } from '../../helpers';

describe('decision info', () => {
  let propsData;
  let wrapper;

  const createPropsData = ({ decision, isWithdrawing = false }) => ({
    decision,
    decisionDetails: { decisionDetails: 'decisionDetails' },
    headerKey: 'headerKey',
    isWithdrawing,
  });

  const mountDecisionInfo = () => mount(DecisionInfo, { propsData });

  describe('your decision', () => {
    let yourDecision;

    beforeEach(() => {
      propsData = createPropsData({ decision: DECISION_OPT_IN });
      wrapper = mount(DecisionInfo, { propsData });
      yourDecision = wrapper.find(YourDecision);
    });

    it('will exist', () => {
      expect(yourDecision.exists()).toEqual(true);
    });

    it('will pass the received properties', () => {
      expect(yourDecision.props()).toEqual(propsData);
    });
  });

  describe('withdrawing', () => {
    it('will exist when is withdrawing', () => {
      propsData = createPropsData({ decision: DECISION_OPT_IN, isWithdrawing: true });
      wrapper = mountDecisionInfo();
      expect(wrapper.find(Withdrawing).exists()).toEqual(true);
    });

    it('will not exist when not withdrawing', () => {
      propsData = createPropsData({ decision: DECISION_OPT_IN, isWithdrawing: false });
      wrapper = mountDecisionInfo();
      expect(wrapper.find(Withdrawing).exists()).toEqual(false);
    });
  });

  describe('decision info', () => {
    it('will be displayed with an opt-in decision', () => {
      propsData = createPropsData({ decision: DECISION_OPT_IN });
      wrapper = mountDecisionInfo();
      expect(wrapper.find(DecisionDetails).exists()).toEqual(true);
    });

    it('will not be displayed with an opt-out decision', () => {
      propsData = createPropsData({ decision: DECISION_OPT_OUT });
      wrapper = mountDecisionInfo();
      expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
    });
  });
});
