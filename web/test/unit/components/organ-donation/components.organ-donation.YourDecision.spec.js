import YourDecision from '@/components/organ-donation/YourDecision';
import { DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import { mount } from '../../helpers';

const mountYourDecision = ({ headerKey, decision = DECISION_OPT_IN }) =>
  mount(YourDecision, {
    propsData: {
      decision,
      headerKey,
    },
  });

describe('your decision', () => {
  let wrapper;

  describe('default header key', () => {
    beforeEach(() => {
      wrapper = mountYourDecision({});
    });

    it('will translate the header using the default header key', () => {
      expect(wrapper.text())
        .toContain('translate_organDonation.reviewYourDecision.yourDecision.subheader');
    });
  });

  describe('custom header key', () => {
    beforeEach(() => {
      wrapper = mountYourDecision({
        headerKey: 'fruity',
      });
    });

    it('will translate the header using the specified header key', () => {
      expect(wrapper.text())
        .toContain('translate_fruity');
    });
  });
});
