import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import i18n from '@/plugins/i18n';
import NextSteps from '@/components/organ-donation/NextSteps';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import StillYourDecision from '@/components/organ-donation/StillYourDecision';
import ViewDecision from '@/pages/organ-donation/view-decision';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  initialState,
  STATE_OK,
  STATE_CONFLICTED,
} from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('view decision', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = () => ({
    organDonation: initialState(),
    device: {
      isNativeApp: true,
    },
  });

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(ViewDecision, { $store: store, $style, mountOpts: { i18n } });
  };

  describe('not conflicted', () => {
    let state;

    beforeEach(() => {
      state = createState();
      state.organDonation.registration.decision = DECISION_OPT_IN;
      state.organDonation.registration.identifier = '12345';
      state.organDonation.registration.state = STATE_OK;
      $store = createStore({ state });
      wrapper = mountWrapper();
    });

    it('will show your decision', () => {
      expect(wrapper.find(YourDecision).exists()).toBe(true);
    });

    describe('StillYourDecision', () => {
      let stillYourDecision;

      beforeEach(() => {
        stillYourDecision = wrapper.find(StillYourDecision);
      });

      it('will show "StillYourDecision" component', () => {
        expect(stillYourDecision.exists()).toEqual(true);
      });

      it('will set the properties on "StillYourDecision" correctly', () => {
        expect(stillYourDecision.props().showAmend).toEqual(true);
        expect(stillYourDecision.props().showReaffirm).toEqual(false);
      });
    });

    it('will show next steps', () => {
      expect(wrapper.find(NextSteps).exists()).toBe(true);
    });

    it('will show the success message text', () => {
      expect(wrapper.text()).toContain('Your decision has been recorded');
    });

    it('will not show the Decision submitted dialog text', () => {
      expect(wrapper.text()).not.toContain('Decision submitted');
    });

    it('will not show the Decision submitted message text', () => {
      expect(wrapper.text()).not.toContain('We have successfully received your organ donation decision.');
    });

    it('will show other things to do', () => {
      expect(wrapper.find(OtherThingsToDo).exists()).toBe(true);
    });

    describe('selected all organs', () => {
      beforeEach(() => {
        state.organDonation.registration.decision = DECISION_OPT_IN;
        state.organDonation.registration.decisionDetails.all = true;
        wrapper = mountWrapper();
      });

      it('will show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(true);
      });

      it('will show the faith details module', () => {
        expect(wrapper.find(FaithDetailsRegistered).exists()).toBe(true);
      });
    });

    describe('selected some organs', () => {
      beforeEach(() => {
        state.organDonation.registration.decision = DECISION_OPT_IN;
        state.organDonation.registration.decisionDetails.all = false;
        $store.getters['organDonation/isSomeOrgans'] = true;
        wrapper = mountWrapper();
      });

      it('will show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(true);
      });

      it('will show the faith details module', () => {
        expect(wrapper.find(FaithDetailsRegistered).exists()).toBe(true);
      });
    });

    describe('opt out', () => {
      beforeEach(() => {
        state.organDonation.registration.decision = DECISION_OPT_OUT;
        state.organDonation.registration.decisionDetails.all = false;
        wrapper = mountWrapper();
      });

      it('will not show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
      });

      it('will not show the faith details module', () => {
        expect(wrapper.find(FaithDetailsRegistered).exists()).toBe(false);
      });
    });
  });

  describe('conflicted', () => {
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
      expect(wrapper.text()).toContain('Decision submitted');
    });

    it('will show the Decision submitted message text', () => {
      expect(wrapper.text()).toContain('We have successfully received your organ donation decision.');
    });

    it('will not show the success message text', () => {
      expect(wrapper.text()).not.toContain('Your decision has been recorded');
    });

    it('will not show your decision', () => {
      expect(wrapper.find(YourDecision).exists()).toBe(false);
    });

    it('will not show the faith details module', () => {
      expect(wrapper.find(FaithDetailsRegistered).exists()).toBe(false);
    });

    it('will not show next steps', () => {
      expect(wrapper.find(NextSteps).exists()).toBe(false);
    });

    it('will not show the decision details', () => {
      expect(wrapper.find(DecisionDetails).exists()).toEqual(false);
    });

    it('will not show still your decision', () => {
      expect(wrapper.find(StillYourDecision).exists()).toEqual(false);
    });

    it('will show other things to do', () => {
      expect(wrapper.find(OtherThingsToDo).exists()).toBe(true);
    });
  });
});
