import AppointedRepIcon from '@/components/icons/organ-donation/AppointedRepIcon';
import i18n from '@/plugins/i18n';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import YesIcon from '@/components/icons/organ-donation/YesIcon';
import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_APPOINTED_REP, DECISION_OPT_IN, DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';
import { mount } from '../../helpers';

const mountYourDecision = ({
  headerKey,
  decision = DECISION_OPT_IN,
  all = true,
  isWithdrawing = false,
}) =>
  mount(YourDecision, {
    propsData: {
      decision,
      decisionDetails: { all },
      headerKey,
      isWithdrawing,
    },
    $style: {
      'appointedrep-label': 'appointedrep-label',
      icon: 'icon',
      'optin-label': 'optin-label',
      'optout-label': 'optout-label',
      'withdraw-label': 'withdraw-label',
    },
    mountOpts: { i18n },
  });

describe('your decision', () => {
  let wrapper;

  describe('default header key', () => {
    beforeEach(() => {
      wrapper = mountYourDecision({});
    });

    it('will translate the header using the default header key', () => {
      expect(wrapper.text())
        .toContain('Your decision');
    });
  });

  describe('custom header key', () => {
    beforeEach(() => {
      wrapper = mountYourDecision({
        headerKey: 'organDonation.button.iDoNotWantToDonate.header',
      });
    });

    it('will translate the header using the specified header key', () => {
      expect(wrapper.text()).toContain('NO');
    });
  });

  describe('withdrawing', () => {
    beforeEach(() => {
      wrapper = mountYourDecision({ decision: DECISION_APPOINTED_REP, isWithdrawing: true });
    });

    it('will not have an icon', () => {
      expect(wrapper.find('.icon').exists()).toBe(false);
    });

    describe('label', () => {
      let label;

      beforeEach(() => {
        label = wrapper.find('.withdraw-label');
      });

      it('will exist', () => {
        expect(label.exists()).toBe(true);
      });

      it('will display the withdraw decision text', () => {
        expect(label.text()).toBe('Withdraw my decision from the register');
      });
    });
  });

  describe('decision', () => {
    describe('appointed rep', () => {
      beforeEach(() => {
        wrapper = mountYourDecision({ decision: DECISION_APPOINTED_REP });
      });

      it('will have appointed rep icon', () => {
        const { icon } = wrapper.vm;
        expect(icon).toBe(AppointedRepIcon);
      });

      describe('label', () => {
        let label;

        beforeEach(() => {
          label = wrapper.find('.appointedrep-label');
        });

        it('will exist', () => {
          expect(label.exists()).toBe(true);
        });

        it('will display the appointed representative decision text', () => {
          expect(label.text()).toBe('I have appointed a representative');
        });
      });
    });

    describe('opt in all', () => {
      beforeEach(() => {
        wrapper = mountYourDecision({ decision: DECISION_OPT_IN, all: true });
      });

      it('will have yes icon', () => {
        const { icon } = wrapper.vm;
        expect(icon).toBe(YesIcon);
      });

      describe('label', () => {
        let label;

        beforeEach(() => {
          label = wrapper.find('.optin-label');
        });

        it('will exist', () => {
          expect(label.exists()).toBe(true);
        });

        it('will display the opt in to all decision text', () => {
          expect(label.text()).toBe('Yes I want to donate my organs');
        });
      });
    });

    describe('opt in some', () => {
      beforeEach(() => {
        wrapper = mountYourDecision({ decision: DECISION_OPT_IN, all: false });
      });

      it('will have yes icon', () => {
        const { icon } = wrapper.vm;
        expect(icon).toBe(YesIcon);
      });

      describe('label', () => {
        let label;

        beforeEach(() => {
          label = wrapper.find('.optin-label');
        });

        it('will exist', () => {
          expect(label.exists()).toBe(true);
        });

        it('will display the opt in to some decision text', () => {
          expect(label.text()).toBe('Yes I want to donate my organs');
        });
      });
    });

    describe('opt out', () => {
      beforeEach(() => {
        wrapper = mountYourDecision({ decision: DECISION_OPT_OUT });
      });

      it('will have no icon', () => {
        const { icon } = wrapper.vm;
        expect(icon).toBe(NoIcon);
      });

      describe('label', () => {
        let label;

        beforeEach(() => {
          label = wrapper.find('.optout-label');
        });

        it('will exist', () => {
          expect(label.exists()).toBe(true);
        });

        it('will display the opt out decision text', () => {
          expect(label.text()).toBe('No I do not want to donate my organs');
        });
      });
    });
  });
});
